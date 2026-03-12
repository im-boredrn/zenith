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

    { EnumDamageType.SlashingAttack, DomainEnum.Hemorrhage },
    { EnumDamageType.PiercingAttack, DomainEnum.Hemorrhage },

    {EnumDamageType.Frost, DomainEnum.Cold },

        {EnumDamageType.Suffocation, DomainEnum.Drown }
};
        // Eventually Add Regen Domain unlock for Stage 3

        public bool DebugMode => ZenithSettings.ZDebugMode;
        private EntityPlayer Player => entity as EntityPlayer; // assignment operator is saying assign the value on the left to the value on the right.
        private ZenithSystems systems;

        public ZenithBehavior(Entity entity) : base(entity) // no need to pass Entityplayer entity anymore since we are attaching it to them.
        {     
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


            systems = new ZenithSystems(entity, new ModConfig());
            systems.DomainManager.LoadDomains();           

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
            systems.DomainManager.ProcessDomain(domain, ref damage);
            var domainState = systems.DomainManager.domains[domain];

            Log($"Domain :{domain}\n Tier: {domainState.Tier}\n Counter: {domainState.Counter}/{domainState.Threshold} \n Damage Taken: {damage} ");
            
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

            var domainstate = systems.DomainManager.domains[domain];

            float domainResistance = domainstate.GetResistanceValue();
            float stageMultiplier = systems.ProgressionManager.GetStageMultiplier();
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