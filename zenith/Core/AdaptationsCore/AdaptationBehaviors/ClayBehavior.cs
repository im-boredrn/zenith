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


        

        public bool  HealWithClay(EntityPlayer player,ItemSlot item)
        {
            if (item.Empty) return false;

            var healthBehavior = player.GetBehavior<EntityBehaviorHealth>();

            healthBehavior.Health += state.HealthGain;
            // Make Clay Edible
            // Read HealthGain and Apply to PlayerHealth

            return true;
        }
        
        public void BlockOtherHealing()
        {

        }


    }
}
