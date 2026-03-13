using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;
using zenith.Config;

namespace zenith.Core.Abilities
{
  

    internal class KineticPassive : IPassives
    {

        public void Apply( EntityPlayer entityPlayer) 
        {
            entityPlayer.Properties.KnockbackResistance *= 2f;
            entityPlayer.Properties.FallDamage = false;
        }

      public  void Tick(EntityPlayer entityPlayer)
        {
        }
    }

    internal class ThermalPassive : IPassives
    {
        public void Apply(EntityPlayer entityPlayer)
        {
            
        }
        public void Tick(EntityPlayer entityPlayer)
        {
        }
    }

    internal class ToxicPassive : IPassives
    {
        public void Apply(EntityPlayer entityPlayer)
        { }

        public void Tick(EntityPlayer entityPlayer)
        { }

    }
   
    internal class ColdPassive : IPassives
    {
        public void Apply(EntityPlayer entityPlayer)
        {

        }
        public void Tick(EntityPlayer entityPlayer)
        {
        }
    }
    internal class HemorrhagePassive : IPassives
    {
        public void Apply(EntityPlayer entityPlayer)
        {
          
        }
        public void Tick(EntityPlayer entityPlayer)
        {
            var healthBehavior = entityPlayer.GetBehavior<EntityBehaviorHealth>();
            if (healthBehavior == null) return;



            float regenAmount = ZenithSettings.ZRegenAmount;

            healthBehavior.Health = Math.Min(healthBehavior.MaxHealth, healthBehavior.Health + regenAmount);
            healthBehavior.MarkDirty();
            entityPlayer.World.Logger.Warning($"[DEBUG] RegenAmount: {ZenithSettings.ZRegenAmount}");
        }
    }

    internal class DrownPassive : IPassives
    {
        public void Apply(EntityPlayer entityPlayer)
        { }

        public void Tick(EntityPlayer entityPlayer)
        { }
    }




}
