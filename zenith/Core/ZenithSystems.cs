using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using zenith.Config;
using zenith.Core.Abilities;
using zenith.Core.Domains;
using static zenith.Core.ZenithBehavior;

namespace zenith.Core
{
  
        public class ZenithSystems
        {
        public bool DebugMode => ZenithSettings.ZDebugMode;

        public DomainManager DomainManager { get; }
            public ProgressionManager ProgressionManager { get; }
        public Dictionary<DomainEnum, IPassives> Passives;
        public Dictionary<DomainEnum, IAttackAbilities> Attack;
        private EntityPlayer Player => entity as EntityPlayer;
        private readonly Entity entity;

        public ZenithSystems(Entity entity, ModConfig modConfig)
            {
            this.entity = entity;

            ProgressionManager = new ProgressionManager(entity);

            AbilityFactory abilityFactory = new AbilityFactory(ProgressionManager);

            ProgressionManager.SetFactory(abilityFactory);

           

            Passives = Enum.GetValues<DomainEnum>()
    .Cast<DomainEnum>()
    .Where(d => d != DomainEnum.None) // skip None
    .ToDictionary(d => d, d => abilityFactory.CreatePassives(d));

            Attack = Enum.GetValues(typeof(DomainEnum))
                .Cast<DomainEnum>()
                .Where(d => d != DomainEnum.None)
                .ToDictionary(d => d, d => abilityFactory.CreateAttack(d));
            


            DomainManager = new DomainManager(entity, modConfig);

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


        public void OnServerTick(float dt)
        {
            var player = entity as EntityPlayer;
            if (player == null) return;

            ProgressionManager.TickPassives();
        }
        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }
    }
}
