using System;
using System.Collections.Generic;
using System.Text;
using CreatureType = zenith.Core.Assimilation.AssimilationCore.CreatureType;
namespace zenith.Core.Assimilation.Adaptations
{
    public class CreatureDefinition
    {

        public CreatureType Type { get; set; }


        public string EntityName => Type.ToString();
        public bool HasAdaptation { get; set; }
        public bool AdaptAchieved { get; set; }
        public int Threshold { get; set; }
        public int Counter { get; set; }
        public float NutritionVal { get; set; }
        public bool IsUnknown { get; set; }

        IAdaptation Adaptation { get; }

        public override string ToString()
        {
            return $"Adapted: {AdaptAchieved} | Counter: {Counter}/{Threshold} | Nutrition: {NutritionVal}";
        }

    }
}
