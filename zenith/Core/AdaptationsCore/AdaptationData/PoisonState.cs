using System;
using System.Collections.Generic;
using System.Text;

namespace zenith.Core.AdaptationsCore.AdaptationData
{
    public class PoisonState : AdaptationState
    {

        public float PoisonDamage { get; set; } = 0.1f;
        public int PoisonDuration { get; set; } = 5000;
        public float PoisonResistance { get; set; } = 0.1f;
   //     public float LifeStealGain => PoisonDamage;
        public int MaxPoisonStack { get; set; } = 10;
        public int PoisonStack { get; set; } = 0;
        public float MaxPoison { get; set; } = 100f;
        public float Poison { get; set; } = 0f;
    }
}
