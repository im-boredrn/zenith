using System;
using System.Collections.Generic;
using System.Text;

namespace zenith.Core.Assimilation
{
    public class AssimilationDefinition
    {

       public string EntityName { get; set; }
       public int AssimLVL { get; set; }
        public float MaxLVL { get; set; }
        public float SpeedGain { get; set; }
        public float JumpGain { get; set; }
        public float DamageGain { get; set; }
    }
}
