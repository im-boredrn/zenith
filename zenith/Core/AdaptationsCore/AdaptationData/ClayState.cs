using System;
using System.Collections.Generic;
using System.Text;

namespace zenith.Core.AdaptationsCore.AdaptationData
{
    public sealed class ClayState : AdaptationState
    {

     
        public float HealingReduction { get; set; } = 100;
        public float AbsorbGain { get; set; } = 0.2f; 
        public float OverCharge { get; set; } = 0.5f;
    }
}
