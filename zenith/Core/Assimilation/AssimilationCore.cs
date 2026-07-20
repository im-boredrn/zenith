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
using zenith.Core.Traits;

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
            unknown
        }

        public   Dictionary<CreatureType, AssimilationDefinition> Definitions { get; } =
             new Dictionary<CreatureType, AssimilationDefinition>() 
             {
                 [CreatureType.drifter] = new AssimilationDefinition
                 {
                     EntityName = "drifter",
                     MaxLVL = ZenithSettings.ZAssimGlobalCreatureMaxLVL,
                     DamageGain =0.01f
                 },

                 [CreatureType.bear] = new AssimilationDefinition
                 {
                     EntityName = "bear",
                     MaxLVL = ZenithSettings.ZAssimGlobalCreatureMaxLVL,
                     DamageGain = 0.03f // TODO : Individual Animal Gain Values
                 },

                   [CreatureType.hare] = new AssimilationDefinition
                   {
                       EntityName = "hare",
                       MaxLVL = ZenithSettings.ZAssimGlobalCreatureMaxLVL,
                       JumpGain = 0.01f
                   },
                   
                   [CreatureType.wolf] = new AssimilationDefinition
                   {
                       EntityName = "wolf",
                       MaxLVL = ZenithSettings.ZAssimGlobalCreatureMaxLVL,
                       DamageGain = 0.02f,
                       SpeedGain = 0.01f
                   },

                 [CreatureType.fox] = new AssimilationDefinition
                 {
                     EntityName = "fox",
                     MaxLVL = ZenithSettings.ZAssimGlobalCreatureMaxLVL,
                     SpeedGain = 0.02f
                 },

                 [CreatureType.unknown] = new AssimilationDefinition
                 {
                     EntityName = "?",
                     MaxLVL = ZenithSettings.ZAssimGlobalCreatureMaxLVL,

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
     
         //   Log($"[DATA] Entity Name : {es.GetName()} | Entity Code : {es.Code} | Entity Code Path : {es.Code.Path}");


            var entityName = es.GetName();

            CreatureType type = CreatureClass(es, player);

            if (type == CreatureType.unknown) return;

            Assimilate(entityName, player, type);
            es.WatchedAttributes.SetAttribute("consumed", null); // used to be behind Assimilate ,comment is here just incase of bug.


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


            SendError(serverPlayer, "AssimError", "Unknown Entity!");
            return CreatureType.unknown;
        }



 

        private void Assimilate(string entityName, IServerPlayer player, CreatureType creatureType )
        {

            if (Definitions[creatureType].AssimLVL >= ZenithSettings.ZAssimGlobalCreatureMaxLVL)
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
            }
            return totals;
        }


        private void SaveAssim()
        {
            Log("[FLOW] SaveAssim Called");

            var totals = CalculateTotals();

            foreach (var trait in Definitions.Values)
            {
                var LVLkey = Definitions.Keys + "LVL";
                watchedZenith.SetFloat(LVLkey, trait.AssimLVL);
                watchedZenith.SetFloat(LVLkey, totals.Speed); // load the totals or the derived ??
            }

            entity.WatchedAttributes.MarkPathDirty("zenith");
            // Log($"[SAVE] AssimCounter : {watchedZenith.GetInt("AssimCounter", 0)} | AssimStage : {AssimStage}"); 
        }

        public void LoadAssim()
        {

            foreach (var trait in Definitions.Values)
            {
                var LVLkey = Definitions.Keys + "LVL";

                trait.AssimLVL = watchedZenith.GetFloat(LVLkey, 0);

            }

            foreach (var trait in Definitions.Values)
            {
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