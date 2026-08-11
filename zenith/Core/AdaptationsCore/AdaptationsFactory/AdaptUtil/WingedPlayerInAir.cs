//using System;
//using System.Collections.Generic;
//using System.Text;
//using Vintagestory.API.Common;
//using Vintagestory.API.Common.Entities;
//using Vintagestory.API.Datastructures;
//using Vintagestory.API.MathTools;

//namespace zenith.Core.AdaptationsCore.AdaptationsFactory.AdaptUtil
//{
//    public class WingedPlayerInAir : PModuleInAir
//    {

//        private WingedCore? WingedCore;

//        public override void Initialize(JsonObject config, Entity entity)
//        {
//            base.Initialize(config, entity);

//            WingedCore = entity.World.Api.ModLoader.GetModSystem<WingedCore>();
//        }

//        public override void ApplyFlying(float dt, Entity entity, EntityPos pos, EntityControls controls)
//        {

//            if (WingedCore != null)
//            {
//                WingedCore.ApplyWingedPhysics(dt,entity,pos,controls);
//            }
          
//            else
//            {
//                base.ApplyFlying(dt, entity, pos, controls);
//            }
            
//        }

      
//    }
//}
