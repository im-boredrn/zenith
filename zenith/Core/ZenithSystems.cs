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
using zenith.Core.Inventory;
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

        private AssimilationInventory ServerAssimilationInventory;
        private AssimilationInventory ClientAssimilationInventory;

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

            if (capi == null)
            {
                ServerAssimilationInventory = new AssimilationInventory(1, $"assiminvID-{Player.PlayerUID}", Player.Api);
                ServerAssimilationInventory.LateInitialize($"assiminvID-{Player.PlayerUID}", Player.Api);

            }
            // GUI
            if (capi != null )
            {
                 
                BearSenseRenderer = new BearSenseRenderer(capi, CreatureAdaptations);

                ClientAssimilationInventory = new AssimilationInventory(1,$"assiminvID-{Player.PlayerUID}", capi);
                ClientAssimilationInventory.LateInitialize($"assiminvID-{Player.PlayerUID}", Player.Api);

                ZenithGui = new ZenithGui(capi, DomainManager, AssimilationCore, StatOutput, CreatureAdaptations, ClientAssimilationInventory );
                capi.World.Player.Entity.WatchedAttributes.RegisterModifiedListener("zenith", () =>
                {
                    ZenithGui?.BonusGUI?.UpdateBonusStats();

                    CreatureAdaptations?.ReloadAdapt();



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

        public void OpenAssimInv(IServerPlayer player)
        {
            player.InventoryManager.OpenInventory(ServerAssimilationInventory);
        }
        public void CloseAssimInv(IServerPlayer player)
        {
            player.InventoryManager.CloseInventoryAndSync(ServerAssimilationInventory);
        }

        public void SubmitItemAssim()
        {
            Logger.Log(Player, $"Inventory exists? {ServerAssimilationInventory != null}");

            Logger.Log(Player, $"Slot empty? {ServerAssimilationInventory?[0].Empty}");

            Logger.Log(Player, $"Stack null? {ServerAssimilationInventory?[0].Itemstack == null}");
            if (ServerAssimilationInventory[0].Itemstack == null) return;

            CreatureAdaptations.EatItem(ServerAssimilationInventory[0].Itemstack);
            ServerAssimilationInventory[0].TakeOutWhole();
        }

        public void OnServerTick(float dt)
        {
            if (Player == null) return;

            foreach (var serverTickable in TickManager.ServerTick)
            {
                serverTickable.OnTick(Player,dt);
            }
        }
        public void OnClientTick(float dt)
        {
            if (Player == null) return;
           
            foreach (var tickable in TickManager.ClientTick)
            {
                tickable.OnTick(Player,dt);
            }        
        }
    }
}
