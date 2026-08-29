using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core.AdaptationsCore.AdaptationData;
using zenith.Core.Definitions;
using zenith.Core.Helper;
using static zenith.Core.Assimilation.AssimilationCore;
using CreatureType = zenith.Core.Definitions.CreatureDefinition.CreatureType;
namespace zenith.Core.AdaptationsCore.AdaptationsFactory
{
    public class BearSensesDefinition()  : AdaptationDefinitions()
    {

      
        public override string AdaptationName { get; init; } = "Instinct";
       
    }



}