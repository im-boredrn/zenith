using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core.Progression;

namespace zenith.Core.Abilities
{
  

    internal class KineticPassive : IPassives
    {
        private TreeAttribute watchedZenith;
        private readonly IStageProvider stageProvider;
        public KineticPassive(EntityPlayer entityPlayer, IStageProvider stageProvider)
        {
            watchedZenith = (TreeAttribute)(entityPlayer.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entityPlayer.WatchedAttributes["zenith"] = watchedZenith;

            this.stageProvider = stageProvider;
        }   

        private void LoadStats(EntityPlayer entity)
        {
            watchedZenith = (TreeAttribute)(entity.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entity.WatchedAttributes["zenith"] = watchedZenith;

        }

        private void SaveStats(EntityPlayer player)
        {
            watchedZenith.SetFloat("walkspeedMult", stageProvider.GetSpeedMultiplier());
            player.WatchedAttributes.MarkPathDirty("zenith");
        }

        public void Apply( EntityPlayer entityPlayer) 
        {
            entityPlayer.Properties.KnockbackResistance *= 2f;
            entityPlayer.Properties.FallDamage = false;
            entityPlayer.Stats.Set("walkspeed", "zenith", stageProvider.GetSpeedMultiplier(), true); //Fix loading
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

        private readonly IStageProvider stageProvider;

        public HemorrhagePassive(IStageProvider stageProvider)
        {
            this.stageProvider = stageProvider;
        }

        public void Apply(EntityPlayer entityPlayer)
        { }
        public void Tick(EntityPlayer entityPlayer)
        {
          
            var healthBehavior = entityPlayer.GetBehavior<EntityBehaviorHealth>();
            if (healthBehavior == null) return;
            var zenith = entityPlayer.WatchedAttributes.GetTreeAttribute("zenith");
            var Stage = zenith?.GetInt("Stage", 1);

            float halfHealth = healthBehavior.MaxHealth / 2f;

            // Only continue if health is below or equal to half
            if (healthBehavior.Health > halfHealth  && Stage < 3)
            {
                return;
            }



            // Apply regen
            float regenAmount = ZenithSettings.ZRegenAmount * stageProvider.GetStageMultiplier() ;
            healthBehavior.Health = Math.Min(healthBehavior.MaxHealth, healthBehavior.Health + regenAmount);
            healthBehavior.MarkDirty();
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
