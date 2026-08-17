using System;
using System.Collections.Generic;
using System.Text;

namespace zenith.Core.AdaptationsCore.AdaptationData
{
    public sealed class ClayState : AdaptationState
    {

     
        public float HealingReduction { get; set; } = 100;
        public float HealthGain { get; set; } = 0.2f;
    }
}
