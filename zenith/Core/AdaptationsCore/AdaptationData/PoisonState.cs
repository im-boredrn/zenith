using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;

namespace zenith.Core.AdaptationsCore.AdaptationData
{
    public class PoisonState : AdaptationState
    {

        public float PoisonDamage { get; set; } = 2.1f;
        public float PoisonResistance { get; set; } = 0.1f;
   //     public float LifeStealGain => PoisonDamage;
        public int MaxPoisonStack { get; set; } = 10;
        public float MaxPoison { get; set; } = 100f;
        public float Poison { get; set; } = 0f;

    }
}
