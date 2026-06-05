using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using zenith.Config;
using zenith.Core.Abilities;
using zenith.Core.Domains;
using zenith.Core.Progression;
using zenith.GUI;
using static zenith.Core.ZenithBehavior;

namespace zenith.Core
{
  
        public class ZenithSystems
        {
        public bool DebugMode => ZenithSettings.ZDebugMode;

        public DomainManager DomainManager { get; }
            public ProgressionManager ProgressionManager { get; }
        public ZenithGui ZenithGui { get; }
        public DomainDetailsGUI DomainDetailsGUI { get; }

        public Dictionary<DomainEnum, IPassives> Passives;
        public Dictionary<DomainEnum, IAttackAbilities> Attack;
        private EntityPlayer Player => entity as EntityPlayer;
        private readonly Entity entity;
        private  IDomainInfo domainInfo;

        public ZenithSystems(Entity entity, ModConfig modConfig, ICoreClientAPI capi)
            {
            this.entity = entity;

            // Core managers
            ProgressionManager = new ProgressionManager(entity);
            AbilityFactory abilityFactory = new AbilityFactory(ProgressionManager); // Abstract
            ProgressionManager.SetFactory(abilityFactory);
            ProgressionManager.LoadProgression();
            DomainManager = new DomainManager(entity, modConfig);
            DomainManager.LoadDomains();

            
           
            // GUI
            if (capi != null)
            {
                ZenithGui = new ZenithGui(capi, ProgressionManager, DomainManager); // Abstract
            }

            Passives = Enum.GetValues<DomainEnum>()
    .Cast<DomainEnum>()
    .Where(d => d != DomainEnum.None) // skip None
    .ToDictionary(d => d, d => abilityFactory.CreatePassives(d));

            Attack = Enum.GetValues(typeof(DomainEnum))
                .Cast<DomainEnum>()
                .Where(d => d != DomainEnum.None)
                .ToDictionary(d => d, d => abilityFactory.CreateAttack(d));

         //   bool isClient = capi != null;

            
                WireEvents();
            
            }


        bool eventsWired = false;
            void WireEvents()
            {
            if (eventsWired) 
            {
                Log("[FLOW] Events Already Wired Returning...");
                return;
            }
            eventsWired = true;


            foreach ( var domain in DomainManager.Domains.Values) // per domain instance wire domain maxed event
            {
                domain.DomainMaxed += ProgressionManager.HandleDomainMaxed;
                domain.DomainMaxed += () =>
                {
                    // These are 2 examples of how Action can be used

                    Log($"[EVENT] DomainMaxed ZenithGui UpdateStats Called...");
                    ZenithGui.UpdateStats(); //() INVOKES - immediately call the method once it reaches this line of code
                };

                domain.OnTierUp += (d) => // Action <d> so the parameters contain the object
                {
                    DomainEnum domain = d.GetDomain();
                    ZenithGui?.UpdateStats();
                    Log($"[EVENT] {domain} tier increased to {d.GetTier()}");
                };

            }




            ProgressionManager.OnStageUp += () =>
            {
                Log($"[EVENT] StageUp ZenithGui UpdateStats Called...");
                ZenithGui?.UpdateStats();
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
