using System;
using System.Collections.Generic;
using System.Text;
using zenith.Core.AdaptationsCore.AdaptationsFactory;
namespace zenith.Core.Definitions
{
    public class CreatureDefinition
    {

        public CreatureType Type { get; set; }

        public enum CreatureType
        {
            drifter,
            bowtorn,
            shiver,
            bear,
            hare,
            wolf,
            fox,
            goat,
            deer,
            raccoon,
            sheep,
            chicken,
            pig,
            hyena,
            unknown
        }

        public bool HasAdaptation { get; init; }
        public int Threshold { get; init; }
        public float NutritionVal { get; init; }
        public bool IsUnknown { get; init; }

        public static Dictionary<CreatureType, CreatureDefinition> CreatureDefinitions { get; } = new Dictionary<CreatureType, CreatureDefinition>()
        {
            [CreatureType.drifter] = new CreatureDefinition()
            {
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 0.1f,
                Type = CreatureType.drifter
            },

            [CreatureType.bowtorn] = new CreatureDefinition()
            {
                HasAdaptation = false, // maybe add bone spear ability later somehow.
                Threshold = 5,
                NutritionVal = 0.1f,
                Type = CreatureType.bowtorn
            },

            [CreatureType.shiver] = new CreatureDefinition()
            {
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 0.1f,
                Type = CreatureType.shiver
            },

            [CreatureType.bear] = new CreatureDefinition()
            {
                HasAdaptation = true,
                Threshold = 4,
                NutritionVal = 5f,
                Type = CreatureType.bear,
                AdaptationType = typeof(BearSensesDefinition)
            },

            [CreatureType.hare] = new CreatureDefinition()
            {
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 2,
                Type = CreatureType.hare

            },
            [CreatureType.wolf] = new CreatureDefinition()
            {
                HasAdaptation = true,
                Threshold = 5,
                NutritionVal = 2.5f,
                Type = CreatureType.wolf,
                AdaptationType = typeof(WolfDefinition)
            },

            [CreatureType.fox] = new CreatureDefinition()
            {
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 2,
                Type = CreatureType.fox
            },

            [CreatureType.goat] = new CreatureDefinition()
            {
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 3f,
                Type = CreatureType.goat
            },

            [CreatureType.deer] = new CreatureDefinition()
            {
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 3.5f,
                Type = CreatureType.deer
            },

            [CreatureType.raccoon] = new CreatureDefinition()
            {
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 0.5f,
                Type = CreatureType.raccoon
            },

            [CreatureType.sheep] = new CreatureDefinition()
            {
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 4.5f,
                Type = CreatureType.sheep
            },

            [CreatureType.chicken] = new CreatureDefinition() // Add Flight Adaptatiion -- Glide -- Investigate wingsuit thing 
            {
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 2.5f,
                Type = CreatureType.chicken
            },

            [CreatureType.pig] = new CreatureDefinition()
            {
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 8f,
                Type = CreatureType.pig
            },

            [CreatureType.hyena] = new CreatureDefinition()
            {
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 1.5f,
                Type = CreatureType.hyena
            },

            [CreatureType.unknown] = new CreatureDefinition()
            {
                IsUnknown = true
            },
        };

        public static IReadOnlyDictionary<CreatureType, CreatureDefinition> CreatureLibrary => CreatureDefinitions;

        public Type AdaptationType { get; init; }
       

    }
}
