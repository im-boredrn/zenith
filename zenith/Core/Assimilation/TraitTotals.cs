using System;
using System.Collections.Generic;
using System.Text;
using StatType = zenith.Core.Assimilation.StatOutput.StatType;
namespace zenith.Core.Assimilation
{
    public class TraitTotals
    {
        private readonly Dictionary<StatType, float> values = new();


        public float this[StatType stat]
        {
            get => values.GetValueOrDefault(stat, 0f);
            set => values[stat] = value;
        }

      
    }
}
