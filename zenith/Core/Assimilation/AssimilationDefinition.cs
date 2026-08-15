using System;
using System.Collections.Generic;
using System.Text;
using zenith.Config;
using StatType = zenith.Core.Assimilation.StatOutput.StatType;
using CreatureType = zenith.Core.Adaptations.CreatureDefinition.CreatureType;
namespace zenith.Core.Assimilation
{
    public class AssimilationDefinition
    {

       public string EntityName { get; set; }
       public int AssimLVL { get; set; }
        public float MaxLVL { get; set; }
        public bool IsUnknown { get; set; }

        public  Dictionary<StatType, float> Gains { get; set; } = [];
        



        public static Dictionary<CreatureType, AssimilationDefinition> Definitions { get; } =
             new()
             {

                 [CreatureType.drifter] = new()
                 {
                     EntityName = "drifter",
                     MaxLVL = ZenithSettings.ZDrifterCreatureMaxLVL,
                     Gains =
                     {
                         [StatType.Strength] = 0.03f
                     }
                 },

                 [CreatureType.bowtorn] = new()
                 {
                     EntityName = "bowtorn",
                     MaxLVL = ZenithSettings.ZBowtornCreatureMaxLVL,
                     Gains =
                     {
                         [StatType.Strength] = 0.03f,
                     }
                 },

                 [CreatureType.shiver] = new()
                 {
                     EntityName = "shiver",
                     MaxLVL = ZenithSettings.ZShiverCreatureMaxLVL,
                     Gains =
                     {
                         [StatType.Strength] = 0.03f,
                         [StatType.Speed] = 0.01f
                     }
                 },

                 [CreatureType.bear] = new()
                 {
                     EntityName = "bear",
                     MaxLVL = ZenithSettings.ZBearCreatureMaxLVL,

                     Gains =
                     {
                         [StatType.Strength] = 0.1f,
                         [StatType.Health] = 0.1f,
                         [StatType.AnimalLoot] = 0.1f
                     }


                 },

                 [CreatureType.hare] = new()
                 {
                     EntityName = "hare",
                     MaxLVL = ZenithSettings.ZHareCreatureMaxLVL,
                     Gains =
                       {
                           [StatType.Jump] = 0.02f,
                           [StatType.CropRate] = 0.04f
                       }
                 },

                 [CreatureType.wolf] = new()
                 {
                     EntityName = "wolf",
                     MaxLVL = ZenithSettings.ZWolfCreatureMaxLVL,
                     Gains =
                       {
                           [StatType.Strength] = 0.02f,
                           [StatType.Speed] = 0.01f,
                           [StatType.Harvesting] = 0.03f
                       }

                 },

                 [CreatureType.fox] = new()
                 {
                     EntityName = "fox",
                     MaxLVL = ZenithSettings.ZFoxCreatureMaxLVL,
                     Gains =
                     {
                         [StatType.Speed] = 0.03f,
                         [StatType.Stealth] = 0.01f
                     }
                 },


                 [CreatureType.goat] = new()
                 {
                     EntityName = "goat",
                     MaxLVL = ZenithSettings.ZGoatCreatureMaxLVL,
                     Gains =
                     {
                         [StatType.Jump] = 0.1f,
                         [StatType.Harvesting] = 0.08f
                     }
                 },

                 [CreatureType.deer] = new()
                 {
                     EntityName = "deer",
                     MaxLVL = ZenithSettings.ZDeerCreatureMaxLVL,
                     Gains =
                     {
                         [StatType.Speed] = 0.08f,
                         [StatType.Stealth] = 0.04f
                     }
                 },

                 [CreatureType.raccoon] = new()
                 {
                     EntityName = "raccoon",
                     MaxLVL = ZenithSettings.ZRaccoonCreatureMaxLVL,
                     Gains =
                     {
                         [StatType.Speed] = 0.02f,
                         [StatType.Forage] = 0.05f
                     }
                 },

                 [CreatureType.sheep] = new()
                 {
                     EntityName = "sheep",
                     MaxLVL = ZenithSettings.ZSheepCreatureMaxLVL,
                     Gains =
                     {
                         [StatType.Harvesting] = 0.03f,
                         [StatType.Forage] = 0.04f
                     }
                 },

                 [CreatureType.chicken] = new()
                 {
                     EntityName = "chicken",
                     MaxLVL = ZenithSettings.ZChickenCreatureMaxLVL,
                     Gains =
                     {
                     [StatType.Speed] = 0.01f,
                     [StatType.CropRate] = 0.05f
                     }
                 },

                 [CreatureType.pig] = new()
                 {
                     EntityName = "pig",
                     MaxLVL = ZenithSettings.ZPigCreatureMaxLVL,
                     Gains =
                     {
                         [StatType.Forage] = 0.05f,
                         [StatType.Strength] = 0.01f
                     }
                 },

                 [CreatureType.hyena] = new()
                 {
                     EntityName = "hyena",
                     MaxLVL = ZenithSettings.ZHyenaCreatureMaxLVL,

                     Gains =
                     {
                         [StatType.Speed] = 0.05f,
                         [StatType.Strength] = 0.06f,
                         [StatType.Harvesting] = 0.05f,
                         [StatType.AnimalLoot] = 0.05f
                     }

                 },

                 [CreatureType.unknown] = new ()
                 {
                     EntityName = "?",
                     IsUnknown = true
                 }



        };
    }                                                               
}
