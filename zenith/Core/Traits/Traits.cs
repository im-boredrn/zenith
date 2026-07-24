using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core.Assimilation;
using zenith.Core.Progression;
using zenith.GUI;

namespace zenith.Core.Traits
{
    public class Traits
    {

        private readonly TreeAttribute watchedZenith;
        public static bool DebugMode => ZenithSettings.ZDebugMode;

        private readonly Entity entity;
        private readonly AssimilationCore assimilationProvider;
        private EntityPlayer Player => entity as EntityPlayer;
        private StatOutput StatOutput;
      
        public Traits(Entity entity, AssimilationCore assimilationProvider, StatOutput statOutput)
        {
            this.entity = entity;
            this.assimilationProvider = assimilationProvider;
            this.StatOutput = statOutput;
            var entityPlayer = Player as EntityPlayer;

            watchedZenith = (TreeAttribute)(entityPlayer.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entityPlayer.WatchedAttributes["zenith"] = watchedZenith;
        }

        public static class ZenithKeys
        {
            public const string Speed = "SPD";
            public const string Strength = "DMG";
            public const string Jump = "JHM";

            public const string Health = "MHP";
            public const string Harvesting = "AHT";
            public const string AnimalLoot = "ALD";

            public const string SpeedOutput = "spdo";
            public const string StrengthOutput = "dmgo";
            public const string JumpOutput = "jhmo";

            public const string HealthOutput = "mhpo";
            public const string HarvestingOutput = "ahto";
            public const string AnimalLootOutput = "aldo";



        }

        private GUITotals GetGUITotals()
        {
            var gTotals = new GUITotals();
            var totals = assimilationProvider.CalculateTotals();
    
            foreach (var trait in assimilationProvider.Definitions.Values)
            {
                foreach (var gain in trait.Gains)
                {
                    gTotals.GUIStats[gain.Key] = totals[gain.Key] * StatOutput.OutputPercentages[gain.Key];
                }
            }

            return gTotals  ;
        }

        public void ApplyTraits()
        {
            Log("[FLOW] ApplyTraits Called");


            var totals = assimilationProvider.CalculateTotals();

            foreach (var trait in assimilationProvider.Definitions.Values)
            {
                foreach (var gain in trait.Gains)
                {
                    totals[gain.Key] = totals[gain.Key] * StatOutput.OutputPercentages[gain.Key]/100f;
                }
            }

            float jumpBonus = totals[StatOutput.StatType.Jump] + ZenithSettings.ZInitialJump ; 


            var entityPlayer = Player as EntityPlayer;

            //Combat
            entityPlayer.Stats.Set("meleeWeaponsDamage", "zenith", totals[StatOutput.StatType.Strength], true);
            entityPlayer.Stats.Set("maxhealthExtraPoints", "zenith", totals[StatOutput.StatType.Health], true);


            //Mobility

            entityPlayer.Stats.Set("walkspeed", "zenith", totals[StatOutput.StatType.Speed], true);
            entityPlayer.Stats.Set("jumpHeightMul", "zenith", jumpBonus, true);


            //Utility
            entityPlayer.Stats.Set("animalLootDropRate", "zenith", totals[StatOutput.StatType.ANLoot], true);
            entityPlayer.Stats.Set("animalHarvestingTime", "zenith", totals[StatOutput.StatType.Harvesting] * -1, true);

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
            watchedZenith.SetFloat(ZenithKeys.Strength, gTotals.GUIStats[StatOutput.StatType.Strength]);
            watchedZenith.SetFloat(ZenithKeys.Health, gTotals.GUIStats[StatOutput.StatType.Health]);

            //Mobility

            watchedZenith.SetFloat(ZenithKeys.Speed, gTotals.GUIStats[StatOutput.StatType.Speed]);
            watchedZenith.SetFloat(ZenithKeys.Jump, gTotals.GUIStats[StatOutput.StatType.Jump] + 0.30f);

            //Utility
            watchedZenith.SetFloat(ZenithKeys.AnimalLoot, gTotals.GUIStats[StatOutput.StatType.ANLoot]);
            watchedZenith.SetFloat(ZenithKeys.Harvesting, gTotals.GUIStats[StatOutput.StatType.Harvesting]);


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
