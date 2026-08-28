using CompactExifLib;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core.AdaptationsCore.Adaptation_Definitions;
using zenith.Core.AdaptationsCore.AdaptationBehaviors;
using zenith.Core.AdaptationsCore.AdaptationsFactory;
using zenith.Core.Domains;
using zenith.Core.Helper;
using zenith.Core.Inventory;
using zenith.Core.Progression;

namespace zenith.Core
{
    public class ZenithBehaviorClient : EntityBehavior
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

        static readonly Dictionary<EnumDamageType, DomainEnum> DamageDomainMap =
    new()
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

        private EntityPlayer Player => entity as EntityPlayer; // assignment operator is saying assign the value on the left to the value on the right.
        public ZenithSystemsClient Clientsystems;
        public ZenithSystemsServer ServerSystems;
        public ZenithBehavior(Entity entity) : base(entity) // Remember to Split into ZenithBehaviorClient And Server
        {
            if (entity.World.Side == EnumAppSide.Server) // If server Side
            {
                var sapi = entity.World.Api as ICoreServerAPI;
                sapi?.Logger.Warning("Zenith behavior attached to SERVER");


                ServerSystems = new ZenithSystemsServer(entity, new ModConfig()); // pass null for capi


                // Hook health behavior
                var healthBehavior = entity.GetBehavior<EntityBehaviorHealth>();
                
                    healthBehavior?.onDamaged += (damage, source) =>
                    {
                        return ReduceDamage(damage, source);
                    };
                

                // Register server tick listener
                sapi.Event.RegisterGameTickListener(dt => ServerSystems.OnServerTick(dt), 1000);
               
            }
            else if (entity.World.Side == EnumAppSide.Client)
            {
                var capi = entity.World.Api as ICoreClientAPI;
                capi?.Logger.Notification("Zenith behavior attached to CLIENT");
                capi.Event.RegisterGameTickListener(dt => Clientsystems.OnClientTick(dt), 1000);
                // Client-only systems (GUI)
                Clientsystems = new ZenithSystemsClient(entity, new ModConfig(), capi);
            }


        }

        public override void OnEntityReceiveDamage(DamageSource damageSource, ref float damage)
        {
            EnumDamageType type = GetDamageType(damageSource); // catches returned type
            DomainEnum domain = IdentifyDomain(type); // translates type into domain

            var clay = ServerSystems?.Adaptations?.Get<ClayDefinition>();

            if (clay.IsUnlocked && damageSource.Type == EnumDamageType.Heal && damageSource.Source != EnumDamageSource.Revive)
            {
                ClayBehavior.BlockOtherHealing(Player,damage);
            }

            if (domain == DomainEnum.None )
            {
               // Logger.Log(Player,"[DATA] Returning domain Not found  ");
                return;
            }
            ServerSystems.DomainManager.ProcessDomain(domain, ref damage);
            
        }

        public override void OnEntityReceiveSaturation(float saturation, EnumFoodCategory foodCat = EnumFoodCategory.Unknown, float saturationLossDelay = 10, float nutritionGainMultiplier = 1)
        {
            var clay = ServerSystems?.Adaptations?.Get<ClayDefinition>();
            if (clay.IsUnlocked)
            {
                if (clay.Behavior is ClayBehavior clayBehavior)
                {
                    clayBehavior.BlockSaturation(Player, ref saturation);
                    base.OnEntityReceiveSaturation(saturation, foodCat, saturationLossDelay, nutritionGainMultiplier);
                }
              
            }
            else
                base.OnEntityReceiveSaturation(saturation, foodCat, saturationLossDelay, nutritionGainMultiplier);
        }

        public override void DidAttack(DamageSource source, EntityAgent targetEntity, ref EnumHandling handled)
        {
            var poison = ServerSystems?.Adaptations?.Get<PoisonDefinition>(); // Needs Both

            if (poison.IsUnlocked)
            {
                if (poison.Behavior is PoisonBehavior poisonBehavior)
                {
                    poisonBehavior.PoisonInfusion(targetEntity);
                }
            }

            base.DidAttack(source, targetEntity, ref handled);
        }

        private static EnumDamageType GetDamageType(DamageSource source) // #Extractor
        {
            return source.Type;
        }

    
        public static DomainEnum IdentifyDomain(EnumDamageType type) // #Translator
        {
            
            if(DamageDomainMap.TryGetValue(type, out DomainEnum domain))
            {
                return domain;
            }

            return DomainEnum.None;
        }

        public float ReduceDamage(float damage, DamageSource dmgSource)
        {
            
            EnumDamageType type = GetDamageType(dmgSource);
            DomainEnum domain = IdentifyDomain(type);

            if (domain == DomainEnum.None) return damage;

            var domainBody = ServerSystems.DomainManager.Domains[domain];

            float domainResistance = domainBody.DomainBehavior.Resistance();
            float stageMultiplier = Clientsystems.ProgressionManager.GetResistanceMultiplier();
            float finalResistance = domainResistance * stageMultiplier;

            damage /= (1f + finalResistance);
            

            return damage;

        }

      

        public override string PropertyName()
        {
            return "Zenith";
        }
    } 
} 