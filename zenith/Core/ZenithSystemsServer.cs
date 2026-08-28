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
  
    public class ZenithSystemsServer
    {

        public ZenithData ZenithData { get; }
        public DomainManager DomainManager { get; }
            public ProgressionManager ProgressionManager { get; }
        public ZenithGui ZenithGui { get; }
        public BearSenseRenderer BearSenseRenderer { get; }

        public AssimilationCore AssimilationCore { get; }
        public Adaptations Adaptations { get; }
        public Traits.Traits Traits { get; }
        public StatOutput StatOutput { get; }
        private EntityPlayer Player => entity as EntityPlayer;
        private readonly Entity entity;

        private readonly AssimilationInventory ServerAssimilationInventory;

        public ZenithSystemsServer(Entity entity, ModConfig modConfig )
            {
            this.entity = entity;
            ZenithData = new ZenithData(entity);
            // Core managers
            ProgressionManager = new ProgressionManager(entity, ZenithData);

            ProgressionManager.LoadProgression();

            AssimilationCore = new AssimilationCore(entity, ZenithData);
            Adaptations = new Adaptations(entity, ZenithData);
                StatOutput = new StatOutput(entity, ZenithData); // needs capi

            Traits = new Traits.Traits(entity, AssimilationCore, StatOutput, ZenithData);

            if (entity.World.Side == EnumAppSide.Server)
            {
                Traits.ApplyTraits();
            }

            DomainManager = new DomainManager(entity, modConfig, ZenithData);
            DomainManager.LoadDomains();

           
            
                ServerAssimilationInventory = new AssimilationInventory(1, $"assiminvID-{Player.PlayerUID}", Player.Api);
                ServerAssimilationInventory.LateInitialize($"assiminvID-{Player.PlayerUID}", Player.Api);

            
          

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
                domain.DomainBehavior.DomainMaxed += () =>
                {
                    ProgressionManager.HandleDomainMaxed();
                    Logger.Log(Player, $"[EVENT] DomainMaxed ZenithGui UpdateStats Called...");
                };






                AssimilationCore.OnAssimChanged += () =>
                {
                    Traits.ApplyTraits();
                };

                AssimilationCore.AssimilationSuccess += (creatureT) =>
                {
                    Adaptations?.CheckAdaptation(creatureT);
                    Adaptations?.AssimilateLink(creatureT);
                };



                StatOutput.OnOutputChange += () =>
                {
                    Traits.ApplyTraits();
                    Logger.Log(Player, "[EVENT]OUTPUT CHANGE EVENT FIRED");
                };
            }
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
       
            if (ServerAssimilationInventory[0].Itemstack == null) return;

            Adaptations.EatItem(ServerAssimilationInventory[0].Itemstack);
            ServerAssimilationInventory[0].TakeOutWhole();
        }

        public void OnServerTick(float dt)
        {
            if (Player == null) return;

            var snapshot = TickManager.ServerTick.ToArray();

            foreach (var serverTickable in snapshot)
            {
                serverTickable.OnTick(Player,dt);
            }
        }
    }
}
