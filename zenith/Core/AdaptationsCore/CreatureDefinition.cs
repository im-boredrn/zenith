using System;
using System.Collections.Generic;
using System.Text;
using zenith.Core.AdaptationsCore.AdaptationsFactory;
namespace zenith.Core.Adaptations
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

        public string EntityName => Type.ToString();
        public bool HasAdaptation { get; set; }
        public bool IsLocked { get; set; } = false;
        public int Threshold { get; set; }
        public int Counter { get; set; }
        public float NutritionVal { get; set; }
        public bool IsUnknown { get; set; }

        public static Dictionary<CreatureType, CreatureDefinition> CreatureDefinitions { get; } = new Dictionary<CreatureType, CreatureDefinition>()
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
                NutritionVal = 5f,
                AdaptationType = typeof(BearSenses)
            },


            [CreatureType.hare] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 2,
            },

            [CreatureType.wolf] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = true,
                Threshold = 5,
                NutritionVal = 2.5f,
                AdaptationType = typeof(WolfAdaptation)

            },

            [CreatureType.fox] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 2,

            },

            [CreatureType.goat] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 3f,

            },

            [CreatureType.deer] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 3.5f,

            },

            [CreatureType.raccoon] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 0.5f,

            },

            [CreatureType.sheep] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 4.5f,

            },

            [CreatureType.chicken] = new CreatureDefinition() // Add Flight Adaptatiion -- Glide -- Investigate wingsuit thing 
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 2.5f,

            },

            [CreatureType.pig] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 8f,


            },

            [CreatureType.hyena] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 1.5f,

            },




            [CreatureType.unknown] = new CreatureDefinition()
            {
                IsUnknown = true
            },
        };

        public static IReadOnlyDictionary<CreatureType, CreatureDefinition> CreatureLibrary => CreatureDefinitions;

        public Type AdaptationType { get; set; }
        public override string ToString()
        {
            return $"Adapted: {IsLocked} | Counter: {Counter}/{Threshold} | Nutrition: {NutritionVal}";
        }

    }
}
