using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using zenith.Config;
using zenith.Core.Abilities;
using static zenith.Core.ZenithBehavior;

namespace zenith.Core
{
  
        public class ZenithSystems
        {
        public bool DebugMode => ZenithSettings.ZDebugMode;

        public DomainManager DomainManager { get; }
            public ProgressionManager ProgressionManager { get; }
        public Dictionary<DomainEnum, IPassives> Passives;
        private EntityPlayer Player => entity as EntityPlayer;
        private readonly Entity entity;

        public ZenithSystems(Entity entity, ModConfig modConfig)
            {
            this.entity = entity;


            DomainManager = new DomainManager(entity, modConfig);
                ProgressionManager = new ProgressionManager(entity);

            Passives = new Dictionary<DomainEnum, IPassives>()
    {
        { DomainEnum.Kinetic, new KineticAbilities() },
        { DomainEnum.Thermal, new ThermalAbilities() },
        { DomainEnum.Cold, new ColdAbilities() },
         {DomainEnum.Hemorrhage, new HemorrhageAbilities() }
    };

            WireEvents();
            }

            void WireEvents()
            {
            DomainManager.DomainMaxed += ProgressionManager.HandleDomainMaxed;
            DomainManager.TierUp += (d) =>
            {
                DomainEnum domain = d.Domain;

                Log($"[EVENT] {domain} tier increased to {d.Tier}");
            };
        }

        public void TickPassives()
        {
            foreach (var passive in Passives.Values)
            {
                passive.Tick(Player); // called every server tick
            }
        }
        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }
    }
}
