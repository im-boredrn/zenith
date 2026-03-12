using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace zenith.Core.Abilities
{

  
    internal class ThermalAttack : IAttackAbilities
    {
        ProgressionManager ProgressionManager;  
        public void OnAttack(DamageSource source, EntityAgent targetEntity, ref EnumHandling handled)
        {
            float baseChance = 0.2f; // 20% chance on stage 1
            float stageMult = ProgressionManager.GetStageMultiplier();

            float finalChance = Math.Min(1f, baseChance * stageMult); // max 100%
            if (targetEntity.World.Rand.NextDouble() < finalChance)
            {
                targetEntity.Ignite();
            }
         }
    }
}
