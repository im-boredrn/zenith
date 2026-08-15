using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using zenith.Config;
using zenith.Core.AdaptationsCore.AdaptationData;
using zenith.Core.Definitions;
using static zenith.Core.Assimilation.AssimilationCore;
using CreatureType = zenith.Core.Definitions.CreatureDefinition.CreatureType;
namespace zenith.Core.AdaptationsCore.AdaptationsFactory
{
    public class WolfAdaptation(IWorldAccessor world, Entity entity,
        IReadOnlyDictionary<string, AdaptationProgress> statReq) : AdaptationDefinitions(world,entity) 
    {



       

        public override void OnAssimilate(Entity entity , CreatureDefinition creatureDefinition,
            IReadOnlyDictionary<CreatureType, CreatureDefinition> creatureDef)
        {


            
            float Sat;
            Sat = creatureDefinition.NutritionVal * 100f;

            var player = entity as EntityPlayer;
            
            player.ReceiveSaturation(Sat, EnumFoodCategory.Protein, 10f, 2f);
        }

     
        public override CreatureType SourceCreature { get; init; } = CreatureType.wolf;
        public override AdaptCategory.AdaptationCategory AdaptationCategory { get; init; } = AdaptCategory.AdaptationCategory.Creature;
        public override string AdaptationName { get; init; } = "Corpse Consumption";
        public override string AdaptationDescription { get; init; } = "You've gained the ability to digest assimilated creatures";
        public override string LockedDescription { get; init; } = $"Assimilate {statReq["wolf"].Counter}/{CreatureDefinition.CreatureDefinitions[CreatureType.wolf].Threshold} Wolves to unlock ";
    }


}

   

