using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;
using zenith.Config;

namespace zenith.Core
{
  
        public class ZenithSystems
        {
            public DomainManager Domains { get; }
            public ProgressionManager Progression { get; }
            public IAbilitiesManager Abilities { get; }

            public ZenithSystems(Entity entity, ModConfig modConfig)
            {
                Domains = new DomainManager(entity);
                Progression = new ProgressionManager(entity);
            IAbilitiesManager abilitiesManager = new IAbilitiesManager;
            Abilities = abilitiesManager;

                WireEvents();
            }

            void WireEvents()
            {
                Domains.DomainMaxed += Progression.HandleDomainMaxed;
                Progression.OnStageUp += Abilities.Apply;
            }
        }
}
