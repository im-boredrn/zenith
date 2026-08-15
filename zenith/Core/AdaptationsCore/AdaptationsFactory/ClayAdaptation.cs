using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using zenith.Core.AdaptationsCore.AdaptationData;
using zenith.Core.Definitions;
using zenith.Core.Helper;
using static zenith.Core.Definitions.CreatureDefinition;
using AdaptationCategory = zenith.Core.Definitions.BlockDefinitions.BlockCategory;

namespace zenith.Core.AdaptationsCore.AdaptationsFactory
{
    public class ClayAdaptation(IWorldAccessor world, Entity entity,
        IReadOnlyDictionary<string, AdaptationProgress> blockReq) : AdaptationDefinitions (world,entity)
    {

        private EntityPlayer Player => entity as EntityPlayer;

      
        private void HealWithClay()//Behavior
        {

        }

        private void BlockOtherHealing()//Behavior
        {

        }


        // Kinda Stale somewhere aka Hover text not updating
        public override BlockDefinitions.BlockCategory BlockCategory =>
            BlockDefinitions.BlockCategory.Clay;
        public override AdaptCategory.AdaptationCategory AdaptationCategory => AdaptCategory.AdaptationCategory.Block;
        public override string BlockCode { get; init; } = "clay";
        public override string AdaptationName { get; init; } = "Clay Physiology";
        public override string AdaptationDescription { get; init; } = "Regenerate With Clay; You cannot heal through other means";
        public override string LockedDescription { get; init; } =
            $"Eat {blockReq["clay"].BlockLVL }/{BlockDefinitions.BlockLibrary["clay"].Threshold} Clay to unlock "; 
    }
}
