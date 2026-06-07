using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using zenith.Config;
using zenith.Core.Progression;
using DomainEnum = zenith.Core.ZenithBehavior.DomainEnum;

namespace zenith.Core.Abilities
{
    public  class AbilityFactory
    {
        private readonly IStageProvider stageProvider;
        private static bool DebugMode => ZenithSettings.ZDebugMode;

        private readonly Entity entity;
        private EntityPlayer Player => entity as EntityPlayer;

        public AbilityFactory(IStageProvider stageProvider, Entity entity)
        {
            this.stageProvider = stageProvider;
            this.entity = entity;
        }



       public  IPassives CreatePassives(DomainEnum domain)
        {

            if (domain == DomainEnum.None)
                return null;
       //     Log("[FLOW] CreatePassives Called " + domain);
            return domain switch
            {
                DomainEnum.Kinetic => new KineticPassive(),
                DomainEnum.Thermal => new ThermalPassive(),
                DomainEnum.Cold => new ColdPassive(),
                DomainEnum.Toxic => new ToxicPassive(),
                DomainEnum.Bleed => new HemorrhagePassive(stageProvider),
                DomainEnum.Drown => new DrownPassive(),
                _ => throw new ArgumentOutOfRangeException(nameof(domain), domain, null)
            };
            
        }

        public  IAttackAbilities CreateAttack(DomainEnum domain)
        {

            if (domain == DomainEnum.None)
                return null;

            return domain switch
            {
              DomainEnum.Kinetic => new KineticAttack(), // Maybe a movement buff
                DomainEnum.Thermal => new ThermalAttack(stageProvider),
                DomainEnum.Cold => new ColdAttack(),
                DomainEnum.Toxic => new ToxicAttack(), // Could Add Poison if it exists
                DomainEnum.Bleed => new HemorrhageAttack(),
                DomainEnum.Drown => new DrownAttack(),
                _ => throw new ArgumentOutOfRangeException(nameof(domain), domain, null)
            };
        }
        // public static IActiveAbilites CreateActives(DomainEnum domain)
        // {
        //    return domain switch
        //   { DomainEnum.Kinetic => new }
        // }



        private bool CanUseAbilities()
        {
            if (stageProvider.GetStage() < 2)
            {
                return false;
            }

            return true;
        }

        public void ApplyPassives(DomainEnum domain)
        {

            if (!CanUseAbilities())
            {
             //   Log($"[DATA] Current Stage is : {stageProvider.GetStage()} | Returning...");
                return;
            }



            var passive = CreatePassives(domain);
            passive?.Apply(Player);

        }

        public void TickPassives() 
        {
           // Log($"[FLOW] Tick Passives Called");

            if (!CanUseAbilities()) 
            {
               // Log($"[DATA] Current Stage is : {stageProvider.GetStage()} | Returning...");
                return;
            }
            foreach (var domain in Enum.GetValues<DomainEnum>().Where(d => d != DomainEnum.None)) 
            {
                //Log($"[DATA] ticking {domain} for : {Player}");
                var passive = CreatePassives(domain);
                passive?.Tick(Player);
            }
        }


        public void HandleAttack(DomainEnum domain, DamageSource source, EntityAgent target)
        {
            if (!CanUseAbilities())
            {
             //  Log($"[DATA] Current Stage is : {stageProvider.GetStage()} | Returning...");
                return;
            }

            var attack = CreateAttack(domain);


            attack?.OnAttack(source, target);
        }

        public void ApplyPassivesForAllDomains()
        {
            foreach (DomainEnum domain in Enum.GetValues<DomainEnum>())
            {
                if (domain == DomainEnum.None) continue;
                ApplyPassives(domain);
            }
        }

        public void ApplyAttackAbilitiesForAllDomains(DamageSource source, EntityAgent targetEntity)
        {

            foreach (DomainEnum domain in Enum.GetValues<DomainEnum>())
            {
                if (domain == DomainEnum.None) continue;


                HandleAttack(domain, source, targetEntity);
            }
        }

        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }

    }
}
