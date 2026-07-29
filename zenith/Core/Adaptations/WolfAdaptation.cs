using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using static zenith.Core.Assimilation.AssimilationCore;
using CreatureType = zenith.Core.Assimilation.AssimilationCore.CreatureType;
namespace zenith.Core.Adaptations
{
    public class WolfAdaptation : Adaptation // Corpse Consumption
    {
        private readonly Entity entity;


        EntityPlayer Player => entity as EntityPlayer;
      public  CreatureType SourceCreature { get; }

      

       

        public override void OnAssimilate(Entity entity , CreatureDefinition creatureDefinition,
            IReadOnlyDictionary<CreatureType, CreatureDefinition> creatureDef)
        {

            if (!creatureDef[CreatureType.wolf].AdaptAchieved) return;


            float Sat;
            Sat = creatureDefinition.NutritionVal * 100f;

            var player = entity as EntityPlayer;
            
            player.ReceiveSaturation(Sat, EnumFoodCategory.Protein, 10f, 2f);
        }

    }

    public class BearAdaptation : Adaptation // Bear Senses , Pack Mule
    {

        public override void OnAssimilate(Entity entity, CreatureDefinition creatureDefinition, IReadOnlyDictionary<CreatureType, CreatureDefinition> creatureDef)
        {
            base.OnAssimilate(entity, creatureDefinition, creatureDef);
        }



    }
}
