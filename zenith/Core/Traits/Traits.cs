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
using StatType = zenith.Core.Assimilation.StatOutput.StatType;
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

            static public Dictionary<StatType, string> GUIKeys { get; } = new Dictionary<StatType, string>()
            {

                [StatType.Speed] = Speed,
                [StatType.Strength] = Strength,
                [StatType.Jump] = Jump,

                [StatType.Health] = Health,
                [StatType.Harvesting] = Harvesting,
                [StatType.AnimalLoot] = AnimalLoot,

                [StatType.Forage] = Forage,
                [StatType.Stealth] = Stealth,
                [StatType.CropRate] = CropRate
            };

            static public Dictionary<StatType, string> GOutputKeys { get; } = new Dictionary<StatType, string>()
            {

                [StatType.Speed] = SpeedOutput,
                [StatType.Strength] = StrengthOutput,
                [StatType.Jump] = JumpOutput,

                [StatType.Health] = HealthOutput,
                [StatType.Harvesting] = HarvestingOutput,
                [StatType.AnimalLoot] = AnimalLootOutput,

                [StatType.Forage] = ForageOutput,
                [StatType.Stealth] = StealthOutput,
                [StatType.CropRate] = CropRateOutput
            };

            public const string Speed = "SPD";
            public const string Strength = "DMG";
            public const string Jump = "JHM";

            public const string Health = "MHP";
            public const string Harvesting = "AHT";
            public const string AnimalLoot = "ALD";

            public const string Forage = "FDR";
            public const string Stealth = "ASR";
            public const string CropRate = "CDR";


            public const string SpeedOutput = "spdo";
            public const string StrengthOutput = "dmgo";
            public const string JumpOutput = "jhmo";

            public const string HealthOutput = "mhpo";
            public const string HarvestingOutput = "ahto";
            public const string AnimalLootOutput = "aldo";

            public const string ForageOutput = "fdro";
            public const string StealthOutput = "asro";
            public const string CropRateOutput = "cdro";



        }

        private GUITotals GetGUITotals()
        {
            var gTotals = new GUITotals();
            var totals = assimilationProvider.CalculateTotals();
    
      

            foreach (StatType stat in Enum.GetValues<StatType>())
            {
                gTotals.GUIStats[stat] = totals[stat] * StatOutput.OutputPercentages[stat];

            }

            return gTotals  ;
        }

        public void ApplyTraits()
        {


            if (entity.World.Side != EnumAppSide.Server)
            {

              
                Log("[ERROR] ApplyTraits called client side!");
                return;

            }
            Log("[FLOW] ApplyTraits Called");

            var totals = assimilationProvider.CalculateTotals();

          

            foreach (StatType stat in Enum.GetValues<StatType>())
            {
                totals[stat] = totals[stat] * StatOutput.OutputPercentages[stat] / 100f;

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
            entityPlayer.Stats.Set("animalLootDropRate", "zenith", totals[StatOutput.StatType.AnimalLoot], true);
            entityPlayer.Stats.Set("animalHarvestingTime", "zenith", totals[StatOutput.StatType.Harvesting] * -1, true);
            entityPlayer.Stats.Set("forageDropRate", "zenith", totals[StatOutput.StatType.Forage], true);
            entityPlayer.Stats.Set("animalSeekingRange", "zenith", totals[StatType.Stealth] * -1, true);
            entityPlayer.Stats.Set("wildCropDropRate", "zenith", totals[StatType.CropRate], true);

            var healthBehavior = Player.GetBehavior<EntityBehaviorHealth>();
            if (healthBehavior != null)
            {
                healthBehavior.MarkDirty();
            }
            
            SaveTraits();
        }

        public void SaveTraits()
        {

            var gTotals = GetGUITotals();



            foreach (var stat in gTotals.GUIStats) 
            {
                watchedZenith.SetFloat(ZenithKeys.GUIKeys[stat.Key], stat.Value);

                watchedZenith.SetFloat(ZenithKeys.GOutputKeys[stat.Key], StatOutput.OutputPercentages[stat.Key]);
                //Log($"[SAVETRAITS]Key : {stat.Key} | Value {stat.Value} | Output {StatOutput.OutputPercentages[stat.Key]}%  ");
            }

           
            entity.WatchedAttributes.MarkPathDirty("zenith");

        }

        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }
    }
}
