using System;
using System.Collections.Generic;
using System.Text;
using StatType = zenith.Core.Assimilation.StatOutput.StatType;
namespace zenith.Core.Assimilation
{
    public class AssimilationDefinition
    {

       public string EntityName { get; set; }
       public int AssimLVL { get; set; }
        public float MaxLVL { get; set; }
        public bool IsUnknown { get; set; }

        public Dictionary<StatType, float> Gains { get; set; } = new();

       
       
    }
}
