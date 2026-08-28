using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
using zenith.Core.AdaptationsCore.AdaptationData;
using zenith.Core.Helper;

namespace zenith.Core.AdaptationsCore.AdaptationBehaviors
{
    public class PoisonBehavior(PoisonState PoisonState): AdaptationBehavior
    {


     

        public void PoisonInfusion( EntityAgent targetEntity)
        {

            if (!targetEntity.HasBehavior<PoisonedEntity>())
            {
                targetEntity.AddBehavior(new PoisonedEntity(targetEntity, PoisonState));
            }


            if (targetEntity.GetBehavior("PoisonedEntity") is PoisonedEntity poisoned) 
            {
                poisoned.PoisonedStack++;

                if (poisoned.PoisonedStack >= PoisonState.MaxPoisonStack)
                {
                    poisoned.PoisonedStack = PoisonState.MaxPoisonStack;
                }

                if (poisoned._disposed)
                {
                    targetEntity.RemoveBehavior(poisoned);
                }
            }

            if (targetEntity.HasBehavior<PoisonedEntity>()) return;

        
        }

      
        public void LifeSteal() { } // call DidAttack/OnInteract
    }
}
