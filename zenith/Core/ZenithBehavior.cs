using CompactExifLib;
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
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core.Abilities;
using zenith.Core.Domains;
using zenith.Core.Progression;

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
            Bleed,
            Drown,
            None

        }

        //Summary: DomainEnum is The Key and sponge is the returned object from said key.
        //Example domains[DomainEnum.Kinetic].Counter     Counter Comes from DomainSponge object


        //#REF Map Created
        static readonly Dictionary<EnumDamageType, DomainEnum> DamageDomainMap =
    new Dictionary<EnumDamageType, DomainEnum>()
{
    { EnumDamageType.BluntAttack, DomainEnum.Kinetic },
    { EnumDamageType.Gravity, DomainEnum.Kinetic },

    { EnumDamageType.Fire, DomainEnum.Thermal },
    { EnumDamageType.Heat, DomainEnum.Thermal },

    { EnumDamageType.Acid, DomainEnum.Toxic },
    { EnumDamageType.Poison, DomainEnum.Toxic },

    { EnumDamageType.SlashingAttack, DomainEnum.Bleed },
    { EnumDamageType.PiercingAttack, DomainEnum.Bleed },

    {EnumDamageType.Frost, DomainEnum.Cold },

        {EnumDamageType.Suffocation, DomainEnum.Drown }
};

      static public bool DebugMode => ZenithSettings.ZDebugMode;
        private EntityPlayer Player => entity as EntityPlayer; // assignment operator is saying assign the value on the left to the value on the right.
        public ZenithSystems systems;
        private readonly IStageProvider stageProvider;
        private  IDomainInfo domainInfo;

        public ZenithBehavior(Entity entity) : base(entity) // no need to pass Entityplayer entity anymore since we are attaching it to them.
        {
            if (entity.World.Side == EnumAppSide.Server) // If server Side
            {
                var sapi = entity.World.Api as ICoreServerAPI;
                sapi?.Logger.Warning("Zenith behavior attached to SERVER");
                // Server-only systems
                systems = new ZenithSystems(entity, new ModConfig(), null); // pass null for capi


                // Hook health behavior
                var healthBehavior = entity.GetBehavior<EntityBehaviorHealth>();
                if (healthBehavior != null)
                {
                    healthBehavior.onDamaged += (damage, source) =>
                    {
                        return ReduceDamage(damage, source);
                    };
                }

                // Register server tick listener
                sapi.Event.RegisterGameTickListener(dt => systems.OnServerTick(dt), 1000);
            }
            else if (entity.World.Side == EnumAppSide.Client)
            {
                var capi = entity.World.Api as ICoreClientAPI;
                capi?.Logger.Notification("Zenith behavior attached to CLIENT");

                // Client-only systems (GUI)
                systems = new ZenithSystems(entity, new ModConfig(), capi);
            }


            stageProvider = systems.ProgressionManager; 
        }

        public override void DidAttack(DamageSource source, EntityAgent targetEntity, ref EnumHandling handled)
        {

            systems.ApplyAttack(source, targetEntity);
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
            systems.DomainManager.ProcessDomain(domain, ref damage);
            var domainState = systems.DomainManager.Domains[domain]; // Abstract
            domainInfo = systems.DomainManager.Domains[domain];
            Log($"Domain :{domain}\n Tier: {domainInfo.GetTier()}\n Counter: {domainInfo.GetCounter()}/{domainState.GetThreshold()} \n Damage Taken: {damage} ");
            
        }
       
        private EnumDamageType GetDamageType(DamageSource source) // #Extractor
        {
            return source.Type;
        }

    
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

            var domainstate = systems.DomainManager.Domains[domain];

            float domainResistance = domainstate.GetResistanceValue();
            float stageMultiplier = stageProvider.GetResistanceMultiplier();
            float finalResistance = domainResistance * stageMultiplier;

            damage = damage / (1f + finalResistance);
            
            Log("[EXIT] Finished Calling ReduceDamage");

            return damage;

            
        }

 

    

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