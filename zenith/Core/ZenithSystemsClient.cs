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
using static zenith.Core.ZenithBehaviorServer;

namespace zenith.Core
{
  
    public class ZenithSystemsClient
    {

        public ZenithData ZenithData { get; }
        public DomainManager DomainManager { get; } // Separate GUI
            public ProgressionManager ProgressionManager { get; } // Separate GUI
        public ZenithGui ZenithGui { get; }
        public BearSenseRenderer BearSenseRenderer { get; }

        public AssimilationCore AssimilationCore { get; } // Separate GUI
        public Adaptations Adaptations { get; } // Separate GUI
        public StatOutput StatOutput { get; } // Separate GUI
        private EntityPlayer Player => entity as EntityPlayer;
        private readonly Entity entity;

        private readonly AssimilationInventory ClientAssimilationInventory;

        public ZenithSystemsClient(Entity entity, ModConfig modConfig, ICoreClientAPI capi)
            {
            this.entity = entity;
            ZenithData = new ZenithData(entity);
            // Core managers
            ProgressionManager = new ProgressionManager(entity, ZenithData);

            ProgressionManager.LoadProgression();

            AssimilationCore = new AssimilationCore(entity, ZenithData);
            Adaptations = new Adaptations(entity, ZenithData);
               // StatOutput = new StatOutput(entity, capi, ZenithData);

            DomainManager = new DomainManager(entity, modConfig, ZenithData);
            DomainManager.LoadDomains();

                 
                BearSenseRenderer = new BearSenseRenderer(capi, Adaptations);

                ClientAssimilationInventory = new AssimilationInventory(1,$"assiminvID-{Player.PlayerUID}", capi);
                ClientAssimilationInventory.LateInitialize($"assiminvID-{Player.PlayerUID}", Player.Api);


            ZenithGui = new ZenithGui(capi, ZenithData, AssimilationCore, StatOutput, Adaptations, ClientAssimilationInventory);

                capi.World.Player.Entity.WatchedAttributes.RegisterModifiedListener("zenith", () =>
                {
                    ZenithGui?.BonusGUI?.UpdateBonusStats();

                    Adaptations?.ReloadAdapt();



                });

            

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
                domain.DomainBehavior.DomainMaxed += ProgressionManager.HandleDomainMaxed;

            }
           

            AssimilationCore?.AssimilationSuccess += (creatureT ) =>
            {
                Adaptations?.CheckAdaptation(creatureT);
                Adaptations?.AssimilateLink(creatureT);
            };



        }


     
        public void OnClientTick(float dt)
        {
            if (Player == null) return;


            var snapshot = TickManager.ClientTick.ToArray();

            foreach (var tickable in snapshot)
            {
                tickable.OnTick(Player,dt);
            }        
        }
    }
}
