using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using zenith.Core.AdaptationsCore;
using zenith.Core.AdaptationsCore.AdaptationData;
using static zenith.Core.Assimilation.AssimilationCore;
using static zenith.Core.Helper.AdaptationsEnum;
using BlockCategory = zenith.Core.Definitions.BlockDefinitions.BlockCategory;
using CreatureType = zenith.Core.Definitions.CreatureDefinition.CreatureType;
namespace zenith.Core.Definitions
{
    public abstract class AdaptationDefinitions()
    {
     
        public abstract string AdaptationName { get; init; }

        public static string GetName(AdaptationEnum adaptation) => adaptation switch
        {
            AdaptationEnum.Wolf => "Corpse Consumption",
            AdaptationEnum.Bear => "Instinct",
            AdaptationEnum.Clay => "Clay Physiology",
            AdaptationEnum.Poison => "Fungal",
            _ => "Unknown"
        };

        public static string GetDescription(AdaptationEnum adaptation) => adaptation switch
        {
            AdaptationEnum.Wolf => "You've gained the ability to digest assimilated creatures",
            AdaptationEnum.Bear => "Enhance your senses to detect Predator and Prey. Toggle With I",
            AdaptationEnum.Clay => "Your body no longer needs food or bandages to heal, absorbing clay in their place." +
            " Other means of healing are ineffective and will instead damage you",
            AdaptationEnum.Poison => "You have an infectious touch",
            _ => "Unknown"
        };

        public static string GetLockedDescription(AdaptationEnum adaptation) => adaptation switch
        {
            AdaptationEnum.Wolf => $"Assimilate {CreatureDefinition.CreatureDefinitions[CreatureType.wolf].Threshold} Wolves to unlock ",
            AdaptationEnum.Bear => $"Assimilate {CreatureDefinition.CreatureDefinitions[CreatureType.bear].Threshold} Bears to unlock ",
            AdaptationEnum.Clay => $"Eat {BlockDefinitions.BlockDefinition["clay"].Threshold} Clay to unlock ",
            AdaptationEnum.Poison => $"Eat {BlockDefinitions.BlockLibrary["mushroom"].Threshold}  mushrooms to unlock",
            _ => "Unknown"
        };

    }
}
