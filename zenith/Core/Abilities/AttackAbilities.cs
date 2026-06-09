using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
using zenith.Core.Progression;

namespace zenith.Core.Abilities
{

  
    internal class ThermalAttack : IAttackAbilities
    {
        private readonly IStageProvider  stageProvider;  

        public ThermalAttack(IStageProvider stageProvider)
        {
            this.stageProvider = stageProvider;
        }

        public void OnAttack(DamageSource source, EntityAgent targetEntity)
        {
            float baseChance = 0.1f; // 10% chance 
            float stageMult = stageProvider.GetIgniteChanceMultiplier(); 

            float finalChance = Math.Min(1f, baseChance + stageMult); // max 100%
            if (targetEntity.World.Rand.NextDouble() < finalChance)
            {
                targetEntity.Ignite();
            }

          //  targetEntity.Stats.Set("healingeffectivness", "Thermal", -0.2f, true);

         }
    }

    internal class HemorrhageAttack : IAttackAbilities
    {
        public void OnAttack(DamageSource source, EntityAgent targetEntity )
        {
            // maybe bleed if it exists
        }
    }

    internal class KineticAttack : IAttackAbilities
    {
        public void OnAttack(DamageSource source, EntityAgent targetEntity)
        { }
    }

    internal class ColdAttack : IAttackAbilities
    {
        public void OnAttack(DamageSource source, EntityAgent targetEntity)
        {
       //     targetEntity.Stats.Set("walkspeed", "cold", -0.1f, true);
        
        
        }
    }
    internal class ToxicAttack : IAttackAbilities
    {
        public void OnAttack(DamageSource source, EntityAgent targetEntity)
        {
     //       targetEntity.Stats.Set("hungerrate", "Toxic", 0.5f, true);
        
        }
    }

    internal class DrownAttack : IAttackAbilities
    {
        public void OnAttack(DamageSource source, EntityAgent targetEntity)
        { }
    }

}
