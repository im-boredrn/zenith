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


        

        public bool CanAbsorb(EntityPlayer player,ItemStack itemStack)
        {
            if (!itemStack.GetName().Contains("clay")) return false;


            if (!state.IsUnlocked)
                return false;

             AbsorbClay(player, itemStack) ;
            return true;
          
        }
        
        public void  AbsorbClay(EntityPlayer player,ItemStack itemStack)
        {


            var healthBehavior = player.GetBehavior<EntityBehaviorHealth>();
            var foodBehavior = player.GetBehavior<EntityBehaviorHunger>();
            
            float totalGain = state.AbsorbGain * itemStack.StackSize;
            
            if (healthBehavior != null)
            {
                healthBehavior.Health += totalGain;

                float healthOverCharge = healthBehavior.MaxHealth * state.OverCharge;

                if (healthBehavior.Health >= healthBehavior.MaxHealth + healthOverCharge)
                {
                    healthBehavior.Health = healthBehavior.MaxHealth + healthOverCharge;
                }
                

                healthBehavior.MarkDirty();
                float current = healthBehavior.Health;

            }


            if (foodBehavior == null) return;

            foodBehavior?.Saturation += totalGain * 100;
            float foodOverCharge = foodBehavior.MaxSaturation * state.OverCharge;

            if (foodBehavior.Saturation >= foodBehavior.MaxSaturation + foodOverCharge)
            {
                foodBehavior.Saturation = foodBehavior.MaxSaturation + foodOverCharge;
            }

        }

        public static void BlockOtherHealing(EntityPlayer player, float damage) // Can use the same method but detect if healing. Or hook into new event like onHeal.
        {



            var healthBehavior = player.GetBehavior<EntityBehaviorHealth>();
            float oldHealth = healthBehavior.Health;
            float newHealth = oldHealth + damage;
            float gainedHealth = newHealth - oldHealth;
            healthBehavior.Health -= gainedHealth;

        }

        public  void BlockSaturation(EntityPlayer player, ref float saturation)
        {
            var foodBehavior = player.GetBehavior<EntityBehaviorHunger>();
            foodBehavior.Saturation  -=  saturation;
            foodBehavior.Saturation += saturation;

        }


    }
}
