//using System;
//using System.Collections.Generic;
//using System.Text;
//using Vintagestory.API.Common;
//using Vintagestory.API.Common.Entities;
//using Vintagestory.GameContent;
//using zenith.Core.Adaptations;
//using zenith.Core.AdaptationsCore.AdaptationsFactory.AdaptUtil;
//using static zenith.Core.Assimilation.AssimilationCore;
//using CreatureType = zenith.Core.Assimilation.AssimilationCore.CreatureType;

//namespace zenith.Core.AdaptationsCore.AdaptationsFactory
//{
//    public class WingedAdaptation(IWorldAccessor world, Entity entity, IReadOnlyDictionary<CreatureType, CreatureDefinition> statReq ) : Adaptation(world,entity)
//    {
//        private EntityPlayer Player => entity as EntityPlayer;

//        public override void Apply()
//        {
//            entity.WatchedAttributes.SetBool("winged", true);
//            entity.Properties.FallDamageMultiplier = 0.05f ;

//            var item = world.GetItem(new AssetLocation("game", "schematic-glider"));

//            if (item == null)
//            {
//                return;
//            }

//            var stack = new ItemStack(item);
//            if (!Player.WatchedAttributes.GetBool("receivedWingBlueprint", false))
//            {
//             world.SpawnItemEntity(stack, Player.Pos.XYZ);
//                Player.WatchedAttributes.SetBool("receivedWingBlueprint", true);
//            }
            

//        }
//        public override CreatureType SourceCreature => CreatureType.chicken;

//        public override string AdaptationName => "Glide"; // Eventually I might make Evolution requirements. I.e. Use the Adaptation X times.
//        public override string AdaptationDescription => "You have reduced glider drag and increased glide; space to flap, K to toggle Wing boost ";
//        public override string LockedDescription => $"Assimilate {statReq[CreatureType.chicken].Counter}/{statReq[CreatureType.chicken].Threshold} Chickens to unlock ";
//    }
//}
