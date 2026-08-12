using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core.AdaptationsCore;
using zenith.Core.AdaptationsCore.AdaptationsFactory;
using zenith.Core.Assimilation;
using zenith.Core.Domains;
using zenith.Core.Helper;
using zenith.Core.Progression;
using zenith.Core.Renderers;
using zenith.Core.Traits;
using zenith.GUI;
using static zenith.Core.ZenithBehavior;

namespace zenith.Core
{
  
    public class ZenithSystems
    {

        public ZenithData ZenithData { get; }
        public DomainManager DomainManager { get; }
            public ProgressionManager ProgressionManager { get; }
        public ZenithGui ZenithGui { get; }
        public BearSenseRenderer BearSenseRenderer { get; }

        public AssimilationCore AssimilationCore { get; }
        public CreatureAdaptations CreatureAdaptations { get; }
       // public WingedEnabler WingedEnabler;
        public Traits.Traits Traits { get; }
        public StatOutput StatOutput { get; }
        private EntityPlayer Player => entity as EntityPlayer;
        private readonly Entity entity;

        public ZenithSystems(Entity entity, ModConfig modConfig, ICoreClientAPI capi)
            {
            this.entity = entity;
            ZenithData = new ZenithData(entity);
            // Core managers
            ProgressionManager = new ProgressionManager(entity, ZenithData);
            // Maybe Replace With Assim : Assimilating Blocks and Entities to give these effects
            ProgressionManager.LoadProgression();

            AssimilationCore = new AssimilationCore(entity, ZenithData);
            CreatureAdaptations = new CreatureAdaptations(entity, ZenithData);
                StatOutput = new StatOutput(entity, capi, ZenithData);

            Traits = new Traits.Traits(entity, AssimilationCore, StatOutput, ZenithData);

            if (entity.World.Side == EnumAppSide.Server)
            {
                Traits.ApplyTraits();
            }

            DomainManager = new DomainManager(entity, modConfig, ZenithData);
            DomainManager.LoadDomains();          

            // GUI
            if (capi != null && Player.Player as ICoreServerAPI == null)
            {
                 
                BearSenseRenderer = new BearSenseRenderer(capi, CreatureAdaptations);


                ZenithGui = new ZenithGui(capi, DomainManager, AssimilationCore, StatOutput, CreatureAdaptations );
                capi.World.Player.Entity.WatchedAttributes.RegisterModifiedListener("zenith", () =>
                {
                    ZenithGui?.BonusGUI?.UpdateBonusStats();

                    CreatureAdaptations?.ReloadAdapt();


                    //if (CreatureAdaptations.ActiveAdaptations.Any(a => a is WingedAdaptation) && WingedEnabler == null)
                    //{
                    //   WingedEnabler = new WingedEnabler(capi, entity as EntityPlayer);
                    //    WingedEnabler.HasWings = true;
                    //}

                });
            }

        WireEvents();
            
            }


        bool eventsWired = false;
        void WireEvents()
        {
            if (eventsWired)
            {
                Logger.Log(Player, "[FLOW] Events Already Wired Returning...");
                return;
            }
            eventsWired = true;


            foreach (var domain in DomainManager.Domains.Values) 
            {
                domain.DomainMaxed += ProgressionManager.HandleDomainMaxed;
                domain.DomainMaxed += () =>
                {
                    Logger.Log(Player,$"[EVENT] DomainMaxed ZenithGui UpdateStats Called...");
                    ZenithGui?.UpdateStats();                
                };

                domain.OnTierUp += (d) => 
                {
                    ZenithGui?.UpdateStats();
                    Logger.Log(Player, $"[EVENT] {domain} tier increased to {d.GetTier()}");

                   
                };

            }


            ProgressionManager.OnStageUp += () =>
            {
                Logger.Log(Player, $"[EVENT] StageUp ZenithGui UpdateStats Called...");
                ZenithGui?.UpdateStats();
                Logger.Log(Player,$"[EVENT] Stats Refreshed!");
                foreach (var domain in DomainManager.Domains.Values)
                {
                    if (domain.GetDomain() == DomainEnum.None) continue;

                }

            };

            ProgressionManager.OnProgressionChanged += () =>
            {
                ZenithGui?.UpdateStats();
            };

            AssimilationCore.OnAssimChanged += () =>
            {
                Traits.ApplyTraits();
            };

            AssimilationCore.AssimilationSuccess += (creatureT ) =>
            {
                CreatureAdaptations?.CheckAdaptation(creatureT);
                CreatureAdaptations?.AssimilateLink(creatureT);
            };

            StatOutput.OnOutputChange += () =>
            {
                ZenithGui?.BonusGUI?.UpdateBonusStats();
                Traits.ApplyTraits();
                Logger.Log(Player, "[EVENT]OUTPUT CHANGE EVENT FIRED");
            };
        }


        public void OnServerTick(float dt)
        {
            var player = entity as EntityPlayer;
            if (player == null) return;

         

            foreach (var serverTickable in TickManager.ServerTick)
            {
                serverTickable.OnTick(player,dt);
            }
        }

        public void OnClientTick(float dt)
        {
            var player = entity as EntityPlayer;
            if (player == null) return;
           
            foreach (var tickable in TickManager.ClientTick)
            {
                tickable.OnTick(player,dt);
            }        
        }
    }
}
