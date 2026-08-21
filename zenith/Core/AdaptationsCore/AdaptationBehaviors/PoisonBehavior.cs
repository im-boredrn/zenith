using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Vintagestory;
using Vintagestory.GameContent;
using zenith.Core.AdaptationsCore.AdaptationData;
using zenith.Core.Helper;

namespace zenith.Core.AdaptationsCore.AdaptationBehaviors
{
    public class PoisonBehavior(PoisonState state): AdaptationBehavior
    {
         Timer Timer;
        public void PoisonInfusion()
        {
            state.PoisonStack++;
            if (state.PoisonStack > state.MaxPoisonStack)
            {
                state.PoisonStack = state.MaxPoisonStack;
            }

            CalculatePoisonDuration();
        }

        public void CalculatePoisonDuration()
        {
            TimerCallback callback = new TimerCallback(CallPoisonDamage);

            Timer = new Timer(CallPoisonDamage, null, state.PoisonDuration, Timeout.Infinite);

        }

        public void CallPoisonDamage(object sta)
        {
            PoisonDamage();
            Timer.Dispose();

        }

        public void PoisonDamage( )
        {

        }

        public void LifeSteal() { } // call DidAttack/OnInteract
    }
}
