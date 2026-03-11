using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;
using zenith.Config;
using static zenith.Core.ZenithBehavior;

namespace zenith.Core
{
  
        public class ZenithSystems
        {
            public DomainManager Domains { get; }
            public ProgressionManager Progression { get; }
        public Dictionary<DomainEnum, IAbilitiesManager> Abilities;

        public ZenithSystems(Entity entity, ModConfig modConfig)
            {
                Domains = new DomainManager(entity, modConfig);
                Progression = new ProgressionManager(entity);

            Abilities = new Dictionary<DomainEnum, IAbilitiesManager>()
    {
        { DomainEnum.Kinetic, new KineticAbilities() },
        { DomainEnum.Thermal, new ThermalAbilities() },
        { DomainEnum.Cold, new ColdAbilities() },
        {DomainEnum.Toxic, new ToxicAbilities() },
         {DomainEnum.Hemorrhage, new HemorrhageAbilities() }
    };

            WireEvents();
            }

            void WireEvents()
            {
                Domains.DomainMaxed += Progression.HandleDomainMaxed;
            }
        }
}
