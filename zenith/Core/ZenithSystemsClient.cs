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
using zenith.Core.AdaptationsCore.AdaptationBehaviors;
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
        public ZenithGui ZenithGui { get; }

        private BearBehavior BearBehavior { get; }
        public BearSenseRenderer BearSenseRenderer { get; }

        private EntityPlayer Player => entity as EntityPlayer;
        private readonly Entity entity;

        private readonly AssimilationInventory ClientAssimilationInventory;

        public ZenithSystemsClient(Entity entity, ICoreClientAPI capi)
            {
            this.entity = entity;
            ZenithData = new ZenithData(entity);


            BearBehavior = new(entity as EntityPlayer);

            BearSenseRenderer = new BearSenseRenderer(capi, BearBehavior, ZenithData);

                ClientAssimilationInventory = new AssimilationInventory(1,$"assiminvID-{Player.PlayerUID}", capi);
                ClientAssimilationInventory.LateInitialize($"assiminvID-{Player.PlayerUID}", Player.Api);


            ZenithGui = new ZenithGui(capi, ZenithData, ClientAssimilationInventory);

                capi.World.Player.Entity.WatchedAttributes.RegisterModifiedListener("zenith", () =>
                {
                    ZenithGui?.BonusGUI?.UpdateBonusStats();

                //    Adaptations?.ReloadAdapt(); // may be unnecessary


                });


            
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
