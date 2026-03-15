using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainEnum = zenith.Core.ZenithBehavior.DomainEnum;

namespace zenith.Core.Abilities
{
    public  class AbilityFactory
    {
        ProgressionManager progressionManager;
        public AbilityFactory(ProgressionManager progressionManager)
        {
            this.progressionManager = progressionManager;
        }

       public  IPassives CreatePassives(DomainEnum domain)
        {

            if (domain == DomainEnum.None)
                return null;

            return domain switch
            {
                DomainEnum.Kinetic => new KineticPassive(),
                DomainEnum.Thermal => new ThermalPassive(),
                DomainEnum.Cold => new ColdPassive(),
                DomainEnum.Toxic => new ToxicPassive(),
                DomainEnum.Bleed => new HemorrhagePassive(progressionManager),
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
              DomainEnum.Kinetic => new KineticAttack(),
                DomainEnum.Thermal => new ThermalAttack(progressionManager),
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
    }
}
