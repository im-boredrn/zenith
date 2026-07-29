using System;
using System.Collections.Generic;
using System.Text;
using CreatureType = zenith.Core.Assimilation.AssimilationCore.CreatureType;
namespace zenith.Core.Adaptations
{
    public class CreatureDefinition
    {

        public CreatureType Type { get; set; }


        public string EntityName => Type.ToString();
        public bool HasAdaptation { get; set; }
        public int Threshold { get; set; }
        public int Counter { get; set; }
        public float NutritionVal { get; set; }
        public bool IsUnknown { get; set; }


        public Type AdaptationType { get; set; }
     //  public IAdaptation Adaptation { get; set; }

        public override string ToString()
        {
            return $"Adapted: {HasAdaptation} | Counter: {Counter}/{Threshold} | Nutrition: {NutritionVal}";
        }

    }
}
