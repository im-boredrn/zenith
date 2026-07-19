using System;
using System.Collections.Generic;
using System.Text;

namespace zenith.Core.Traits
{
    public class AssimilationDefinition
    {

       public string EntityName { get; set; }
       public int AssimLVL { get; set; }
        public int MaxLVL { get; set; }
        public int SpeedGain { get; set; }
        public int JumpGain { get; set; }
        public int DamageGain { get; set; }
    }
}
