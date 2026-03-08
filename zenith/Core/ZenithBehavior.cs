using CompactExifLib;
using HarmonyLib;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using zenith.Config;

namespace zenith.Core
{
    public class ZenithBehavior : EntityBehavior
    {
        public enum DomainEnum
        {
            Kinetic,
            Thermal,
            Cold,
            Toxic,
            Hemorrhage,
            Drown,
            None

        }
        //#REF Dictionary Creation
        Dictionary<DomainEnum, DomainSponge> domains;

        bool TierEventRegistered = false;
        bool StageEventRegistered = false;

        //Summary: DomainEnum is The Key and sponge is the returned object from said key.
        //Example domains[DomainEnum.Kinetic].Counter     Counter Comes from DomainSponge object

        static readonly Dictionary<EnumDamageType, DomainEnum> DamageDomainMap =
    new Dictionary<EnumDamageType, DomainEnum>()
{
    { EnumDamageType.BluntAttack, DomainEnum.Kinetic },
    { EnumDamageType.Gravity, DomainEnum.Kinetic },

    { EnumDamageType.Fire, DomainEnum.Thermal },
    { EnumDamageType.Heat, DomainEnum.Thermal },

    { EnumDamageType.Acid, DomainEnum.Toxic },
    { EnumDamageType.Poison, DomainEnum.Toxic },

    { EnumDamageType.SlashingAttack, DomainEnum.Hemorrhage },
    { EnumDamageType.PiercingAttack, DomainEnum.Hemorrhage },

    {EnumDamageType.Frost, DomainEnum.Cold },

        {EnumDamageType.Suffocation, DomainEnum.Drown }
};
        // Eventually Add Regen Domain unlock for Stage 3

        public bool DebugMode => ZenithSettings.ZDebugMode;
        private EntityPlayer Player => (EntityPlayer)entity; // assignment operator is saying assign the value on the left to the value on the right.


        private DomainManager domainManager;
        private ProgressionManager progressionManager;
        public ZenithBehavior(Entity entity) : base(entity) // no need to pass Entityplayer entity anymore since we are attaching it to them.
        {
            ModConfig config = new ModConfig();

            if (entity.HasBehavior<ZenithBehavior>()) return;      
         
             if (entity.World.Side == EnumAppSide.Client)
            {
                Log($"Current Side {entity.World.Side} returning");
                return;
            }
            else if (entity.World.Side == EnumAppSide.Server)
            {
                (entity.World.Api as ICoreServerAPI)?.Logger.Warning($"Zenith behavior attached to {entity.World.Side}");
            }

     
            // Create Progression Manager With Entity
            ProgressionManager progressionManager = new ProgressionManager(entity);

            DomainManager domainManager = new DomainManager(entity, config);



            // Wire events
            foreach (var pair in domains)
            {
                DomainEnum domain = pair.Key;
                DomainSponge sponge = pair.Value;

                sponge.DomainMaxed += (d) =>
                {
                    progressionManager.HandleDomainMaxed(d); // PM updates stage, points, etc.
                };

                sponge.OnTierUp += (d) =>
                {
                    Log($"[EVENT] {domain} tier increased to {d.Tier}");
                };
            }


            foreach (var domainState in domains) // Loads
            {
                DomainEnum domain = domainState.Key; 
                DomainSponge sponge = domainState.Value;
                string keyCounter = "zenith." + domain + ".counter";
                string keyTier = "zenith." + domain + ".tier";

                sponge.Tier = entity.WatchedAttributes.GetAsInt(keyTier);
                sponge.Counter = entity.WatchedAttributes.GetAsInt(keyCounter);
            }


            EntityBehaviorHealth healthBehavior = entity.GetBehavior<EntityBehaviorHealth>();
            if (healthBehavior != null)
            {

                healthBehavior.onDamaged += (damage, source) =>
                {
                    return ReduceDamage(damage, source);
                };
            }


        }


        public override void OnEntityReceiveDamage(DamageSource damageSource, ref float damage)
        {

            EnumDamageType type = GetDamageType(damageSource); // catches returned type
          
            DomainEnum domain = IdentifyDomain(type); // translates type into domain

            if (domain == DomainEnum.None )
            {
                Log("[DATA] Returning domain Not found  ");
                return;
            }
            domainManager.ProcessDomain(domain, ref damage);

            domainManager.DomainMaxed += progressionManager.HandleDomainMaxed;

               Log($"Domain :{domain}\n {domain} Tier: {domains[domain].Tier} \n {domain} Counter: {domains[domain].Counter}/{domains[domain].Threshold} \n Damage Taken: {damage}");
            
        }
       
        private EnumDamageType GetDamageType(DamageSource source) // #Extractor
        {
            return source.Type;
        }

        private const string BluntDomain = "bluntdamage";
    
        public DomainEnum IdentifyDomain(EnumDamageType type) // #Translator
        {
            
                Log("[FLOW] Calling IdentifyDomain");

            if(DamageDomainMap.TryGetValue(type, out DomainEnum domain))
            {
                Log($"[DATA] Returning {domain} Domain");
                return domain;
            }

            Log("[DATA] Returning No Domain");
            return DomainEnum.None;
        }

        public float ReduceDamage(float damage, DamageSource dmgSource)
        {
            
               Log("[FLOW] Calling ReduceDamage");


            EnumDamageType type = GetDamageType(dmgSource);
            DomainEnum domain = IdentifyDomain(type);

            if (domain == DomainEnum.None) return damage;

            var domainstate = domains[domain];

            damage = domainstate.Resistance(damage);
            Log("[EXIT] Finished Calling ReduceDamage");

            return damage;

            
        }

        int processCalls = 0;
       

       




        /// <summary>
        /// Persists the current state of the domain to ensure that all relevant data is saved. );
        /// </summary>
        /// <remarks>Call this method after updating domain data to guarantee that changes are
        /// properly stored. Ensure that all necessary information is prepared before invoking this
        /// method.</remarks>

      

        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }

        public override string PropertyName()
        {
            return "Zenith";
        }
    } 
} 