using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core.Adaptations;
using StatType = zenith.Core.Assimilation.StatOutput.StatType;

namespace zenith.Core.Assimilation
{

   

    public class AssimilationCore : EntityBehavior, IAssimilationProvider
    {
        static public bool DebugMode => ZenithSettings.ZDebugMode;
        private EntityPlayer Player => entity as EntityPlayer;

        private readonly ZenithData zenithData;


        public enum CreatureType
        {
            drifter,
            bowtorn,
            shiver,
            bear,
            hare,
            wolf,
            fox,
            goat,
            deer,
            raccoon,
            sheep,
            chicken,
            pig,
            hyena,
            unknown
        }

        public   Dictionary<CreatureType, AssimilationDefinition> Definitions { get; } =
             new Dictionary<CreatureType, AssimilationDefinition>() 
             {

                 [CreatureType.drifter] = new AssimilationDefinition
                 {
                     EntityName = "drifter",
                     MaxLVL = ZenithSettings.ZDrifterCreatureMaxLVL,
                     Gains =
                     {
                         [StatType.Strength] = 0.03f
                     }
                 },

                 [CreatureType.bowtorn] = new AssimilationDefinition
                 {
                     EntityName = "bowtorn",
                     MaxLVL = ZenithSettings.ZBowtornCreatureMaxLVL,
                     Gains =
                     {
                         [StatType.Strength] = 0.03f,
                     }
                 },

                 [CreatureType.shiver] = new AssimilationDefinition
                 {
                     EntityName = "shiver",
                     MaxLVL = ZenithSettings.ZShiverCreatureMaxLVL,
                     Gains =
                     {
                         [StatType.Strength] = 0.03f,
                         [StatType.Speed] = 0.01f
                     }
                 },

                 [CreatureType.bear] = new AssimilationDefinition
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

                   [CreatureType.hare] = new AssimilationDefinition
                   {
                       EntityName = "hare",
                       MaxLVL = ZenithSettings.ZHareCreatureMaxLVL,
                       Gains =
                       {
                           [StatType.Jump] = 0.02f,
                           [StatType.CropRate] = 0.04f
                       }
                   },
                   
                   [CreatureType.wolf] = new AssimilationDefinition
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

                 [CreatureType.fox] = new AssimilationDefinition
                 {
                     EntityName = "fox",
                     MaxLVL = ZenithSettings.ZFoxCreatureMaxLVL,
                     Gains =
                     {
                         [StatType.Speed] = 0.03f,
                         [StatType.Stealth] = 0.01f
                     }
                 },


                 [CreatureType.goat] = new AssimilationDefinition
                 {
                     EntityName = "goat",
                     MaxLVL = ZenithSettings.ZGoatCreatureMaxLVL,
                     Gains =
                     {
                         [StatType.Jump] = 0.1f,
                         [StatType.Harvesting] = 0.08f
                     }
                 },

                 [CreatureType.deer] = new AssimilationDefinition
                 {
                     EntityName = "deer",
                     MaxLVL = ZenithSettings.ZDeerCreatureMaxLVL,
                     Gains =
                     {
                         [StatType.Speed] = 0.08f,
                         [StatType.Stealth] = 0.04f
                     }
                 },

                 [CreatureType.raccoon] = new AssimilationDefinition
                 {
                     EntityName = "raccoon",
                     MaxLVL = ZenithSettings.ZRaccoonCreatureMaxLVL,
                     Gains =
                     {
                         [StatType.Speed] = 0.02f,
                         [StatType.Forage] = 0.05f
                     }
                 },

                 [CreatureType.sheep] = new AssimilationDefinition
                 {
                     EntityName = "sheep",
                     MaxLVL = ZenithSettings.ZSheepCreatureMaxLVL,
                     Gains =
                     {
                         [StatType.Harvesting] = 0.03f,
                         [StatType.Forage] = 0.04f
                     }
                 },

                 [CreatureType.chicken] = new AssimilationDefinition
                 {
                     EntityName = "chicken",
                     MaxLVL = ZenithSettings.ZChickenCreatureMaxLVL,
                     Gains =
                     {
                     [StatType.Speed] = 0.01f,
                     [StatType.CropRate] = 0.05f
                     }
                 },

                 [CreatureType.pig] = new AssimilationDefinition
                 {
                     EntityName = "pig",
                     MaxLVL = ZenithSettings.ZPigCreatureMaxLVL,
                     Gains =
                     {
                         [StatType.Forage] = 0.05f,
                         [StatType.Strength] = 0.01f
                     }
                 },

                 [CreatureType.hyena] = new AssimilationDefinition
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

                 [CreatureType.unknown] = new AssimilationDefinition
                 {
                     EntityName = "?",
                     IsUnknown = true
                 }

             };
 
      
        public AssimilationCore(Entity entity, ZenithData data) : base(entity)
        {
            this.zenithData = data;

            LoadAssim();
            CalculateTotals();
            
        }
      public  event Action  OnAssimChanged;
        public event Action<CreatureType> AssimilationSuccess;
        public void TryAssimilate(IServerPlayer player)
        {
            var es = Player?.EntitySelection?.Entity;

           // Log($"[FLOW] Try Assimilate Called!");

            if (es == null)
            {
                player.SendIngameError("AssimError", $"Nothing to Consume!");
                Log($"[DATA] Entity selection is null ");
                return;
            }
            else if (es.Alive)
            {
                player.SendIngameError("AssimError", $"Must Kill!");
                return;
            }


            if (!es.HasBehavior<EntityBehaviorHarvestable>())
            {
                Log($"[DATA] Does not have EntityBehaviorHarvestable");
                return;
            }
            if (es.WatchedAttributes.GetBool("consumed") ) // may not persist - due to it not being loaded
                // its likely only saved on world but not retrieved on next load
                // untested as of implementation of ZenithNetwork
            {
                player.SendIngameError("AssimError", "Already Consumed!");
                return;
            }

            ScanAssimilate(player);

        }

        public void ScanAssimilate(IServerPlayer player)
        {
            var es = Player?.EntitySelection?.Entity;
     
            Log($"[DATA] Entity Name : {es.GetName()} | Entity Code : {es.Code} | Entity Code Path : {es.Code.Path}");


            var entityName = es.GetName();

            CreatureType type = CreatureClass(es, player);

            if (type == CreatureType.unknown) return;

            Assimilate(entityName, player, type);
            es.WatchedAttributes.SetBool("consumed", true); 



            foreach (var trait in Definitions.Values)
            {
              //  Log($"[DATA] Progress : {trait.AssimLVL}/{trait.MaxLVL}");
            }
        }

      


        private CreatureType CreatureClass(Entity entity, IServerPlayer serverPlayer)
        {
            var code = entity.Code.Path;

            foreach (var kvp in Definitions)
            {
                if (code.Contains(kvp.Value.EntityName))
                {
                    return kvp.Key;
                }
            }

            SendError(serverPlayer, "AssimError", "Unknown Entity!");
            return CreatureType.unknown;
        }
 

        private void Assimilate(string entityName, IServerPlayer player, CreatureType creatureType )
        {

            if (Definitions[creatureType].AssimLVL >= Definitions[creatureType].MaxLVL)
            {
                SendError(player, "AssimError", "MaxLevelReached");
                return;
            }


            Definitions[creatureType].AssimLVL += 1 * ZenithSettings.ZAssimCreatureLVLMult; 


            var sapi = entity.World.Api as ICoreServerAPI;


            sapi.SendIngameDiscovery(player, $"AssimDisc {entityName}", $"Assimilated {entityName}");
            AssimilationSuccess?.Invoke(creatureType);
            OnAssimChanged?.Invoke();
            SaveAssim();
        }

       

        public TraitTotals CalculateTotals()
        {
            TraitTotals totals = new TraitTotals();
            foreach (var trait in Definitions.Values)
            {   
                foreach (var gain in trait.Gains)
                {
                    totals[gain.Key] += trait.AssimLVL * gain.Value;
                }
            }
  
            return totals;
        }

        //public override WorldInteraction[] GetInteractionHelp(IClientWorldAccessor world, EntitySelection es, IClientPlayer player, ref EnumHandling handled)
        //{

        //    if (!es.Entity.Alive)
        //    {
        //        return new WorldInteraction[]
        //        {
        //            new WorldInteraction
        //            {
        //                ActionLangCode = "zenith:Assimilate",
        //                RequireFreeHand = false,
        //                HotKeyCode = "V"

        //            }
        //        };


        //    }


        //    return base.GetInteractionHelp(world, es, player, ref handled);
        //}


        private void SaveAssim()
        {
           // Log("[FLOW] SaveAssim Called");


            foreach (var kvp in Definitions)
            {

                var key = kvp.Key;
                var trait = kvp.Value;

                zenithData.Tree.SetInt($"{key}LVL", trait.AssimLVL);
          
             //   Log($"KEY : {key} LVL");
            }
            

            entity.WatchedAttributes.MarkPathDirty("zenith");
            // Log($"[SAVE] AssimCounter : {watchedZenith.GetInt("AssimCounter", 0)} | AssimStage : {AssimStage}"); 
        }

        public void LoadAssim()
        {


            foreach (var kvp in Definitions)
            {
                var key = kvp.Key;
                var trait = kvp.Value;

                trait.AssimLVL = zenithData.Tree.GetInt($"{key}LVL", 0);
   
             //   Log($"[LOAD] AssimLVL : {trait.AssimLVL}/{trait.MaxLVL}");
            }
          
        }

 public float GetCreatureLevel(CreatureType creatureType)
        {
            return Definitions[creatureType].AssimLVL;
        }

      
        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }

        private void SendError(IServerPlayer player, string code, string errorMessage)
        {
            player.SendIngameError(code, errorMessage);
        }

        public override string PropertyName()
        {
            return "Assimilate";
        }
    }
}