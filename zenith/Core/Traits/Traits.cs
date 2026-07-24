using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core.Assimilation;
using zenith.Core.NetWork;
using zenith.Core.Progression;
using zenith.GUI;

namespace zenith.Core.Traits
{
    public class Traits
    {

        private readonly TreeAttribute watchedZenith;
        public static bool DebugMode => ZenithSettings.ZDebugMode;

        private readonly Entity entity;
        private readonly IAssimilationProvider assimilationProvider;
        private EntityPlayer Player => entity as EntityPlayer;
        private StatOutput StatOutput;
      
        public Traits(Entity entity, IAssimilationProvider assimilationProvider, StatOutput statOutput)
        {
            this.entity = entity;
            this.assimilationProvider = assimilationProvider;
            this.StatOutput = statOutput;
            var entityPlayer = Player as EntityPlayer;

            watchedZenith = (TreeAttribute)(entityPlayer.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entityPlayer.WatchedAttributes["zenith"] = watchedZenith;
        }

        private GUITotals GetGUITotals()
        {
            var gTotals = new GUITotals();
            var totals = assimilationProvider.CalculateTotals();
            var gSpeedOutput = StatOutput.OutputPercentages[StatOutput.StatType.Speed] ; 
            var gStrengthOutput = StatOutput.OutputPercentages[StatOutput.StatType.Strength] ;
            var gJumpOutput = StatOutput.OutputPercentages[StatOutput.StatType.Jump];
            var gHealthOutput = StatOutput.OutputPercentages[StatOutput.StatType.Health];
            var gANLootOutput = StatOutput.OutputPercentages[StatOutput.StatType.ANLoot];
            var gHarvestingOutput = StatOutput.OutputPercentages[StatOutput.StatType.Harvesting];


            float jumpBonus = totals.Jump + 0.49f;


            gTotals.GDamage = totals.Damage * gStrengthOutput;
            gTotals.GJump = jumpBonus * gJumpOutput;
            gTotals.GSpeed = totals.Speed * gSpeedOutput; // Ie. 0.05 * 80 = 4 | 80% of 5 = 4
            gTotals.GHealth = totals.Health * gHealthOutput;
            gTotals.GANLoot = totals.ANLoot * gANLootOutput;
            gTotals.GHarvesting = totals.Harvesting * gHarvestingOutput;

            return gTotals  ;
        }

        public void ApplyTraits()
        {
            Log("[FLOW] ApplyTraits Called");
            var speedOutput = StatOutput.OutputPercentages[StatOutput.StatType.Speed]/100f;
            var strengthOutput = StatOutput.OutputPercentages[StatOutput.StatType.Strength]/100f;
            var jumpOutput = StatOutput.OutputPercentages[StatOutput.StatType.Jump]/100f;
            var healthOutput = StatOutput.OutputPercentages[StatOutput.StatType.Health]/ 100f;
            var aNLootOutput = StatOutput.OutputPercentages[StatOutput.StatType.ANLoot]/ 100f;
            var harvestingOutput = StatOutput.OutputPercentages[StatOutput.StatType.Harvesting]/ 100f;


            var totals = assimilationProvider.CalculateTotals();
            float jumpBonus = totals.Jump + 0.30f ;

            float finalJump = (jumpBonus * jumpOutput);
            float finalDamage = (totals.Damage * strengthOutput);
            float finalSpeed = (totals.Speed * speedOutput);
            float finalHealth = (totals.Health * healthOutput);
            float finalANLoot = (totals.ANLoot * aNLootOutput);
            float finalHarvesting = -1f * (totals.Harvesting * harvestingOutput);

            var entityPlayer = Player as EntityPlayer;

            //Combat
            entityPlayer.Stats.Set("meleeWeaponsDamage", "zenith", finalDamage, true);
            entityPlayer.Stats.Set("maxhealthExtraPoints", "zenith", finalHealth, true);


            //Mobility

            entityPlayer.Stats.Set("walkspeed", "zenith", finalSpeed, true);
            entityPlayer.Stats.Set("jumpHeightMul", "zenith", finalJump, true);


            //Utility
            entityPlayer.Stats.Set("animalLootDropRate", "zenith", finalANLoot, true);
            entityPlayer.Stats.Set("animalHarvestingTime", "zenith", finalHarvesting, true);

            var healthBehavior = Player.GetBehavior<EntityBehaviorHealth>();
            if (healthBehavior != null)
            {
                healthBehavior.MarkDirty();
                healthBehavior.MaxHealth = healthBehavior.Health;
            }
            
            SaveTraits();
        }

        public void SaveTraits()
        {

            var gTotals = GetGUITotals();

            //Combat
            watchedZenith.SetFloat("Dmg", gTotals.GDamage);
            watchedZenith.SetFloat("MHP", gTotals.GHealth);

            //Mobility

            watchedZenith.SetFloat("SPD", gTotals.GSpeed);
            watchedZenith.SetFloat("JHM", gTotals.GJump);

            //Utility
            watchedZenith.SetFloat("ALD",gTotals.GANLoot );
            watchedZenith.SetFloat("AHT", gTotals.GHarvesting);


            //   Log($"[SAVE]  | GJump : {gTotals.GJump}\n Damage : {gTotals.GDamage}\n Speed : {gTotals.GSpeed} | NOTE: Output percent behind by 10% ");


            entity.WatchedAttributes.MarkPathDirty("zenith");

        }

        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }
    }
}
