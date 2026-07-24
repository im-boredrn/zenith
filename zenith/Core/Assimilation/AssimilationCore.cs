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

namespace zenith.Core.Assimilation
{

    // Planned Features
    // Eating Mobs to gain traits ie. bunny for higher jumps and foxes for quicker movement.


    public class AssimilationCore : EntityBehavior, IAssimilationProvider
    {
        static public bool DebugMode => ZenithSettings.ZDebugMode;
        private EntityPlayer Player => entity as EntityPlayer;

        private readonly TreeAttribute watchedZenith;

        public enum CreatureType
        {
            drifter,
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
                     DamageGain =0.01f
                 },

                 [CreatureType.bear] = new AssimilationDefinition
                 {
                     EntityName = "bear",
                     MaxLVL = ZenithSettings.ZBearCreatureMaxLVL,
                     DamageGain = 0.15f, 
                     HealthGain = 0.1f,
                     ANLootGain = 0.1f

                 },

                   [CreatureType.hare] = new AssimilationDefinition
                   {
                       EntityName = "hare",
                       MaxLVL = ZenithSettings.ZHareCreatureMaxLVL,
                       JumpGain = 0.02f
                   },
                   
                   [CreatureType.wolf] = new AssimilationDefinition
                   {
                       EntityName = "wolf",
                       MaxLVL = ZenithSettings.ZWolfCreatureMaxLVL,
                       DamageGain = 0.02f,
                       SpeedGain = 0.01f
                   },

                 [CreatureType.fox] = new AssimilationDefinition
                 {
                     EntityName = "fox",
                     MaxLVL = ZenithSettings.ZFoxCreatureMaxLVL,
                     SpeedGain = 0.03f
                 },


                 [CreatureType.goat] = new AssimilationDefinition
                 {
                     EntityName = "goat",
                     MaxLVL = ZenithSettings.ZGoatCreatureMaxLVL,
                     JumpGain = 0.1f
                 },

                 [CreatureType.deer] = new AssimilationDefinition
                 {
                     EntityName = "deer",
                     MaxLVL = ZenithSettings.ZDeerCreatureMaxLVL,
                     SpeedGain = 0.1f
                     // Add SeekingGain
                 },

                 //[CreatureType.raccoon] = new AssimilationDefinition
                 //{
                 //    EntityName = "raccoon",
                 //    MaxLVL = ZenithSettings.ZAssimGlobalCreatureMaxLVL,
                 //    SpeedGain = 0.1f
                 //    // VesselGain
                 //},

                 //[CreatureType.sheep] = new AssimilationDefinition
                 //{
                 //    EntityName = "sheep",
                 //    MaxLVL = ZenithSettings.ZAssimGlobalCreatureMaxLVL,
                 //    SpeedGain = 0.1f
                 //    // hungerrateGain
                 //},

                 //[CreatureType.chicken] = new AssimilationDefinition
                 //{
                 //    EntityName = "chicken",
                 //    MaxLVL = ZenithSettings.ZAssimGlobalCreatureMaxLVL,
                 //    //Add WildCropGain
                 //},

                 //[CreatureType.pig] = new AssimilationDefinition
                 //{
                 //    EntityName = "pig",
                 //    MaxLVL = ZenithSettings.ZAssimGlobalCreatureMaxLVL,
                 //    //Add ForageGain = 0.1f
                 //},

                 [CreatureType.hyena] = new AssimilationDefinition
                 {
                     EntityName = "hyena",
                     MaxLVL = ZenithSettings.ZHyenaCreatureMaxLVL,
                     SpeedGain = 0.05f,
                     DamageGain = 0.08f,
                     HarvestingGain = 0.1f,
                     ANLootGain = 0.1f
                 },

                 [CreatureType.unknown] = new AssimilationDefinition
                 {
                     EntityName = "?",
                 }

             };
 
      
        public AssimilationCore(Entity entity) : base(entity)
        {


            watchedZenith = (TreeAttribute)(entity.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entity.WatchedAttributes["zenith"] = watchedZenith;
            LoadAssim();
            CalculateTotals();
            
        }
      public  event Action  OnAssimChanged;
      
        public void TryAssimilate(IServerPlayer player)
        {
            var es = Player?.EntitySelection?.Entity;

            Log($"[FLOW] Try Assimilate Called!");

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
            if (es.WatchedAttributes.HasAttribute("consumed") ) // may not persist - due to it not being loaded
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
            es.WatchedAttributes.SetAttribute("consumed", null); 


            foreach (var trait in Definitions.Values)
            {
                Log($"[DATA] Progress : {trait.AssimLVL}/{trait.MaxLVL}");
            }
        }

        private CreatureType CreatureClass(Entity entity, IServerPlayer serverPlayer)
        {
            var code = entity.Code.Path;

            if (code.Contains("drifter")) return CreatureType.drifter;

            if (code.Contains("bear")) return CreatureType.bear;

            if (code.Contains("wolf")) return CreatureType.wolf;

            if (code.Contains("fox")) return CreatureType.fox;

            if (code.Contains("hare")) return CreatureType.hare;

            if (code.Contains("goat")) return CreatureType.goat;

            if (code.Contains("deer")) return CreatureType.deer;

            //if (code.Contains("raccoon")) return CreatureType.raccoon;

            //if (code.Contains("sheep")) return CreatureType.sheep;

            //if (code.Contains("chicken")) return CreatureType.chicken;

            //if (code.Contains("pig")) return CreatureType.pig;

            if (code.Contains("hyena")) return CreatureType.hyena;




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
            Definitions[creatureType].AssimLVL += 1 * ZenithSettings.ZAssimCreatureLVLMult; // Foreach level apply whatever bonus the creature has
             // I'll calc the trait gain in Trait Manager or whatever          
            var sapi = entity.World.Api as ICoreServerAPI;


            
            sapi.SendIngameDiscovery(player, $"AssimDisc {entityName}", $"Assimilated {entityName}");
            OnAssimChanged?.Invoke();
            SaveAssim();
        }

        public float GetCreatureLevel(CreatureType creatureType)
        {
            return Definitions[creatureType].AssimLVL;
        }

      

        public TraitTotals CalculateTotals()
        {
            TraitTotals totals = new TraitTotals();

            foreach (var trait in Definitions.Values)
            {
                totals.Speed += trait.AssimLVL * trait.SpeedGain;
                totals.Damage += trait.AssimLVL * trait.DamageGain;
                totals.Jump += trait.AssimLVL * trait.JumpGain;
                totals.Health += trait.AssimLVL * trait.HealthGain;
                totals.ANLoot += trait.AssimLVL * trait.ANLootGain;
                totals.Harvesting += trait.AssimLVL * trait.HarvestingGain;
            }
            return totals;
        }


        private void SaveAssim()
        {
            Log("[FLOW] SaveAssim Called");


            foreach (var kvp in Definitions)
            {

                var key = kvp.Key;
                var trait = kvp.Value;

                watchedZenith.SetInt($"{key}LVL", trait.AssimLVL);
          
                Log($"KEY : {key}");
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

                trait.AssimLVL = watchedZenith.GetInt($"{key}LVL", 0);
   
                Log($"[LOAD] AssimLVL : {trait.AssimLVL}/{trait.MaxLVL}");
            }
          
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