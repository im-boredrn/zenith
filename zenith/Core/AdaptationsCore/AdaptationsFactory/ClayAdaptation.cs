using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using zenith.Core.Adaptations;
using static zenith.Core.Adaptations.CreatureDefinition;
using AdaptationCategory = zenith.Core.AdaptationsCore.BlockDefinitions.BlockCategory;

namespace zenith.Core.AdaptationsCore.AdaptationsFactory
{
    public class ClayAdaptation(IWorldAccessor world, Entity entity,
        IReadOnlyDictionary<AssetLocation, BlockDefinitions> blockReq) : Adaptation (world,entity)
    {


        private void HealWithClay()
        {

        }

        private void BlockOtherHealing()
        {

        }



        public override BlockDefinitions.BlockCategory BlockCategory =>
            BlockDefinitions.BlockCategory.Clay;
        public override AdaptCategory.AdaptationCategory AdaptationCategory => AdaptCategory.AdaptationCategory.Block;
        public override string AdaptationName => "Clay Physiology";
        public override string AdaptationDescription => "Regenerate With Clay; You cannot heal through other means";
        public override string LockedDescription =>
            $"Eat {blockReq[new AssetLocation("game:clay-blue")].BlockLVL }/{blockReq[new AssetLocation("game:clay-blue")].Threshold} Clay to unlock ";

    }
}
