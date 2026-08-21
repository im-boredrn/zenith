using System;
using System.Collections.Generic;
using System.Text;
using zenith.Core.AdaptationsCore.AdaptationData;
using zenith.Core.Definitions;
using static zenith.Core.Definitions.CreatureDefinition;

namespace zenith.Core.AdaptationsCore.Adaptation_Definitions
{
    public class PoisonDefinition(PoisonState state) : AdaptationDefinitions
    {



        public override BlockDefinitions.BlockCategory BlockCategory { get; init; } =
           BlockDefinitions.BlockCategory.Fungal;

        public override AdaptCategory.AdaptationCategory AdaptationCategory { get; init; } = AdaptCategory.AdaptationCategory.Block;
        public override string AdaptationName { get; init; } = "Fungal";
        public override string AdaptationDescription { get; init; } = "You have an infectious touch.";
        public override string LockedDescription => 
            $"Eat {state.BlockLVL}/{BlockDefinitions.BlockLibrary["mushroom"].Threshold}  mushrooms to unlock";
    }
}
