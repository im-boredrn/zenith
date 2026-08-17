using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using zenith.Core.AdaptationsCore.AdaptationData;

namespace zenith.Core.AdaptationsCore.AdaptationBehaviors
{
    public class ClayBehavior( ClayState state) : AdaptationBehavior
    {


        public bool IsClay(EntityPlayer player,ItemStack itemStack)
        {


            if (!state.IsUnlocked)
                return false;


            if (!itemStack.GetName().Contains("clay"))
            {
                if (itemStack.Collectible.GetCollectibleBehavior<CollectibleBehaviorHealingItem>(true)
                    is not CollectibleBehaviorHealingItem healingItem) return false; // Check for food eventually too.


                ClayBehavior.BlockOtherHealing(healingItem); //  Needs testing without clayAdaptation | Needs testing on if Vals actually work.
                
                
                
              
            }


             HealWithClay(player, itemStack) ;
            return true;
          
        }

        public void  HealWithClay(EntityPlayer player,ItemStack itemStack)
        {

            var healthBehavior = player.GetBehavior<EntityBehaviorHealth>();

            float totalGain = state.HealthGain * itemStack.StackSize;
            
            healthBehavior.Health += totalGain;

        }
        
        public static void BlockOtherHealing(CollectibleBehaviorHealingItem item) // Can use the same method but detect if healing. Or hook into new event like onHeal.
        {
            item.ApplicationTimeSec = 9999;
            item.Health = 40;
            return ;
        }


    }
}
