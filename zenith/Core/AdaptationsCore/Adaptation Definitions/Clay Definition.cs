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
    public class ClayDefinition() : AdaptationDefinitions()
    {



        public override string AdaptationName { get; init; } = "Clay Physiology";
      
    }
}
