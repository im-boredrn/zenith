using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using zenith.Config;
using CreatureType = zenith.Core.Assimilation.AssimilationCore.CreatureType;

namespace zenith.Core.Assimilation.Adaptations
{
    public class CreatureAdaptations : EntityBehavior
    {

        public Dictionary<CreatureType, CreatureDefinition> CreatureDefinitions { get; } = new Dictionary<CreatureType, CreatureDefinition>()
        {
            [CreatureType.drifter] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 0.1f
            },

            [CreatureType.bowtorn] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false, // maybe add bone spear ability later somehow.
                Threshold = 5,
                NutritionVal = 0.1f
            },

            [CreatureType.shiver] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 0.1f
            },

            [CreatureType.bear] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = true,
                Threshold = 4,
                NutritionVal = 10f
            },

            [CreatureType.wolf] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = true,
                Threshold = 5,
                NutritionVal = 5f
            },

            [CreatureType.unknown] = new CreatureDefinition()
            {
                IsUnknown = true
            },
        };

        private EntityPlayer Player => entity as EntityPlayer;
        private readonly TreeAttribute watchedZenith;
        static public bool DebugMode => ZenithSettings.ZDebugMode;


        public CreatureAdaptations(Entity entity) : base(entity)
        {

            watchedZenith = (TreeAttribute)(entity.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entity.WatchedAttributes["zenith"] = watchedZenith;
        }

        public void CheckAdaptation(CreatureType creatureType)
        {
         
            {
                CreatureDefinitions[creatureType].Counter += 1;

                if (CreatureDefinitions[creatureType].Counter == CreatureDefinitions[creatureType].Threshold &&
                    CreatureDefinitions[creatureType].HasAdaptation == true)
                {
                    CreatureDefinitions[creatureType].AdaptAchieved = true;
                }

                foreach (var creature in CreatureDefinitions.Where(c => !c.Value.IsUnknown ))
                {

                    Log($"[CA] {creature.ToString()}");
                }
            }
        }

        public bool AdaptAchieved(CreatureType creatureType)
        {
            return CreatureDefinitions[creatureType].AdaptAchieved;
        }
        public void CheckFood(CreatureType creatureType) // Move to Wolf Adapt
        {
            Log("[CA-FLOW] Check Food Called");

            float Sat;
                Sat = CreatureDefinitions[creatureType].NutritionVal * 100f;

            
            Player.ReceiveSaturation(Sat, EnumFoodCategory.Protein, 10f, 2f);
        }

       
        
        


        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }

        public override string PropertyName()
        {
            return "CreatureAdaptations";
        }
    }
}
