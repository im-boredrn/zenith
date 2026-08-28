using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using zenith.Config;
using static System.Net.Mime.MediaTypeNames;
using StatType = zenith.Core.Assimilation.StatOutput.StatType;
using CreatureType = zenith.Core.Definitions.CreatureDefinition.CreatureType;
using zenith.Core.Helper;
using zenith.Core.Definitions;
namespace zenith.Core.Assimilation
{

   

    public class AssimilationCore : EntityBehavior, IAssimilationProvider
    {
        static public bool DebugMode => ZenithSettings.ZDebugMode;
        private EntityPlayer Player => entity as EntityPlayer;

        private readonly ZenithData zenithData;

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
                Logger.Log(Player,$"[DATA] Entity selection is null ");
                return;
            }
            else if (es.Alive)
            {
                player.SendIngameError("AssimError", $"Must Kill!");
                return;
            }


            if (!es.HasBehavior<EntityBehaviorHarvestable>())
            {
                Logger.Log(Player, $"[DATA] Does not have EntityBehaviorHarvestable");
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
     
            Logger.Log(Player,$"[DATA] Entity Name : {es.GetName()} | Entity Code : {es.Code} | Entity Code Path : {es.Code.Path}");


            var entityName = es.GetName();

            CreatureType type = CreatureClass(es, player);

            if (type == CreatureType.unknown) return;

            Assimilate(entityName, player, type);
            es.WatchedAttributes.SetBool("consumed", true); 



            //foreach (var trait in Definitions.Values)
            //{
            //  //  Log($"[DATA] Progress : {trait.AssimLVL}/{trait.MaxLVL}");
            //}
        }

      


        private CreatureType CreatureClass(Entity entity, IServerPlayer serverPlayer)
        {
            var code = entity.Code.Path;

            foreach (var kvp in AssimilationDefinition.Definitions)
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

            if (AssimilationDefinition.Definitions[creatureType].AssimLVL >= AssimilationDefinition.Definitions[creatureType].MaxLVL)
            {
                SendError(player, "AssimError", "MaxLevelReached");
                return;
            }


            AssimilationDefinition.Definitions[creatureType].AssimLVL += 1 * ZenithSettings.ZAssimCreatureLVLMult;
            StringBuilder gainPrint = new();

            var sapi = entity.World.Api as ICoreServerAPI;

            foreach (var gain in AssimilationDefinition.Definitions[creatureType].Gains)
            {

                var trueVal = gain.Value * 100f;
                gainPrint.Append($"+{trueVal}% {gain.Key} ");  
            }
           
            sapi.SendMessage(player, GlobalConstants.GeneralChatGroup,  $"Assimilated {entityName}" , EnumChatType.Notification );

            if (Player.World.Api is ICoreClientAPI capi) // #TODO Client Server sync
            {
                ZenithCore.PlayPlayerSound(capi, "sounds/assimilation/assimed", Player, 0.8f, 1f);
            }
            sapi.SendMessage(player, GlobalConstants.GeneralChatGroup,
                    gainPrint.ToString(), EnumChatType.Notification);
            AssimilationSuccess?.Invoke(creatureType);
            OnAssimChanged?.Invoke();
            SaveAssim();
        }

       

        public TraitTotals CalculateTotals()
        {
            TraitTotals totals = new ();
            foreach (var trait in AssimilationDefinition.Definitions.Values)
            {   
                foreach (var gain in trait.Gains)
                {
                    totals[gain.Key] += trait.AssimLVL * gain.Value;
                }
            }
  
            return totals;
        }

    

        private void SaveAssim()
        {
           // Log("[FLOW] SaveAssim Called");


            foreach (var kvp in AssimilationDefinition.Definitions)
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


            foreach (var kvp in AssimilationDefinition.Definitions)
            {
                var key = kvp.Key;
                var trait = kvp.Value;

                trait.AssimLVL = zenithData.Tree.GetInt($"{key}LVL", 0);
   
            }
          
        }

  
     

        private static void SendError(IServerPlayer player, string code, string errorMessage)
        {
            player.SendIngameError(code, errorMessage);
        }

        public override string PropertyName()
        {
            return "Assimilate";
        }
    }
}