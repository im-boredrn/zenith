using CompactExifLib;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core.AdaptationsCore.Adaptation_Definitions;
using zenith.Core.AdaptationsCore.AdaptationBehaviors;
using zenith.Core.AdaptationsCore.AdaptationsFactory;
using zenith.Core.Domains;
using zenith.Core.Helper;
using zenith.Core.Inventory;
using zenith.Core.Progression;
using DomainEnum = zenith.Core.Helper.DomainMapAndEnum.DomainEnum;

namespace zenith.Core
{
    public class ZenithBehaviorClient : EntityBehavior
    {
       
       
        public ZenithSystemsClient Clientsystems;
        public ZenithBehaviorClient(Entity entity) : base(entity) 
        {
           
                var capi = entity.World.Api as ICoreClientAPI;
                capi?.Logger.Notification("Zenith behavior attached to CLIENT");
                capi.Event.RegisterGameTickListener(dt => Clientsystems.OnClientTick(dt), 1000);
                // Client-only systems (GUI)
                Clientsystems = new ZenithSystemsClient(entity, capi);
          
        }

 
     
   
        public override string PropertyName()
        {
            return "Zenith";
        }
    } 
} 