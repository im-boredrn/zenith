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
        ProgressionManager progressionManager;  

        public ThermalAttack(ProgressionManager progressionManager)
        {
            this.progressionManager = progressionManager;
        }

        public void OnAttack(DamageSource source, EntityAgent targetEntity)
        {
            float baseChance = 0.2f; // 20% chance on stage 1
            float stageMult = progressionManager.GetStageMultiplier();

            float finalChance = Math.Min(1f, baseChance * stageMult); // max 100%
            if (targetEntity.World.Rand.NextDouble() < finalChance)
            {
                targetEntity.Ignite();
            }
         }
    }

    internal class HemorrhageAttack : IAttackAbilities
    {
        public void OnAttack(DamageSource source, EntityAgent targetEntity )
        {

        }
    }

    internal class KineticAttack : IAttackAbilities
    {
        public void OnAttack(DamageSource source, EntityAgent targetEntity)
        {

        }
    }

    internal class ColdAttack : IAttackAbilities
    {
        public void OnAttack(DamageSource source, EntityAgent targetEntity)
        { }
    }
    internal class ToxicAttack : IAttackAbilities
    {
        public void OnAttack(DamageSource source, EntityAgent targetEntity)
        { }
    }

    internal class DrownAttack : IAttackAbilities
    {
        public void OnAttack(DamageSource source, EntityAgent targetEntity)
        { }
    }

}
