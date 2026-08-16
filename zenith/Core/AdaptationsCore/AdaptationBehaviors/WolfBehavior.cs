using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using zenith.Core.Definitions;
using static zenith.Core.Definitions.CreatureDefinition;

namespace zenith.Core.AdaptationsCore.AdaptationBehaviors
{
    public class WolfBehavior(EntityPlayer entity) : AdaptationBehavior
    {

        public  void OnAssimilate(CreatureType creatureType) // Call with keybind
        {
            
            float Sat;
            Sat = CreatureDefinitions[creatureType].NutritionVal * 100f;


            entity.ReceiveSaturation(Sat, EnumFoodCategory.Protein, 10f, 2f);
        }
    }
}
