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
    public class BearSensesDefinition  : AdaptationDefinitions
    {

       
        static public bool DebugMode => ZenithSettings.ZDebugMode;

        private AdaptationState _state;
        public BearSensesDefinition( AdaptationState state) // Bear Sense, Pack Mule
        {
       
            _state = state;
            
            //Log($"[BEAR] Created {this.GetHashCode()}");
        }
      
        public override CreatureType SourceCreature { get; init; } = CreatureType.bear;
        public override AdaptCategory.AdaptationCategory AdaptationCategory { get; init; } = AdaptCategory.AdaptationCategory.Creature;
        public override string AdaptationName { get; init; } = "Instinct";
        public override string AdaptationDescription { get; init; } = "Enhance your senses to detect Predator and Prey. Toggle With I";
        public override string LockedDescription =>
            $"Assimilate {_state.Counter}/{CreatureDefinition.CreatureLibrary[CreatureType.bear].Threshold} Bears to unlock ";
       
    }



}