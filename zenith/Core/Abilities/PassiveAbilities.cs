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
        private readonly TreeAttribute watchedZenith;
        private readonly IStageProvider stageProvider;
        private readonly EntityPlayer entity;
        public static bool DebugMode => ZenithSettings.ZDebugMode;

        public KineticPassive(EntityPlayer entityPlayer, IStageProvider stageProvider)
        {
         
            this.stageProvider = stageProvider;
            this.entity = entityPlayer; // New Line If stats stop working inspect this (07/3/26)
            watchedZenith = (TreeAttribute)(entityPlayer.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entityPlayer.WatchedAttributes["zenith"] = watchedZenith;
        }   

      

      

        public void Apply( EntityPlayer entityPlayer) 
        {
            var playerStats = entityPlayer.Stats;
            entityPlayer.Properties.KnockbackResistance *= 2f;
            entityPlayer.Properties.FallDamage = false;

        //    playerStats.Set("miningSpeedMul", "zenith", stageProvider.GetMiningSpeedMultiplier(), true);

            SaveStats(); // So I can retrieve stats for GUI
            // could also mark dirty here if other method causes issues.
        }

        public void SaveStats()
        {

            foreach (var key in watchedZenith.Keys)
            {
                var attr = watchedZenith[key];

                Log($"KEY={key}");

                if (attr == null)
                {
                    Log($"NULL ATTRIBUTE: {key}");
                }
                else
                {
                    Log($"TYPE={attr.GetType().Name}");
                }
            }
            entity.WatchedAttributes.MarkPathDirty("zenith");


        }

        private void Log(string message)
        {
            if (!DebugMode) return;
            entity.World.Logger.Warning(message);
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
        {

       //     entityPlayer.Stats.Set("hungerrate", "zenith", -0.5f, true);
        
        }

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
        {
            
         }

        public void Tick(EntityPlayer entityPlayer)
        { }
    }




}
