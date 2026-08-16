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
    public class ClayDefinition(AdaptationState state) : AdaptationDefinitions 
    {



        public override BlockDefinitions.BlockCategory BlockCategory { get; init; } =
            BlockDefinitions.BlockCategory.Clay;
        public override AdaptCategory.AdaptationCategory AdaptationCategory { get; init; } = AdaptCategory.AdaptationCategory.Block;
        public override string BlockCode { get; init; } = "clay";
        public override string AdaptationName { get; init; } = "Clay Physiology";
        public override string AdaptationDescription { get; init; } = "Regenerate With Clay; You cannot heal through other means";
        public override string LockedDescription 
        =>
            $"Eat {state.BlockLVL }/{BlockDefinitions.BlockLibrary["clay"].Threshold} Clay to unlock ";  
    }
}
