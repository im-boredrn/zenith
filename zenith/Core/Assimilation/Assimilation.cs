using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

   
    public class Assimilation : EntityBehavior
    {
       static public bool DebugMode => ZenithSettings.ZDebugMode;
        private EntityPlayer Player => entity as EntityPlayer;

        private readonly TreeAttribute watchedZenith;




        public int AssimCounter { get; private set; } // Individual corpses -- Might name it in modconfig "CorpseValueMultiplier" for tiers of enemies ie. 
        //Dead Drifter - 1, Dead deep Drifter - 2 or 1.5, Dead two headed drifter - 5
        // remove static and see if it persists
        static private int Threshold => ZenithSettings.ZAssimThreshold;

        public int AssimStage { get; private set; }
        static private  int MaxStage => ZenithSettings.ZAssimMaxStage;

        public Assimilation(Entity entity) : base(entity)
        {


            watchedZenith = (TreeAttribute)(entity.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entity.WatchedAttributes["zenith"] = watchedZenith;
            LoadAssim();
            
        }
      public  event Action  OnAssimChanged;
        public  event Action OnAssimStageUp;
      
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
     
            Log($"[DATA] Entity Name : {es.GetName()} ");

            

                es.WatchedAttributes.SetAttribute("consumed", null);

            

            var entityName = es.GetName();

            Assimilate(entityName, player);

            Log($"[DATA] Progress : {AssimCounter}/{Threshold}");
        }

        private void Assimilate(string entityName, IServerPlayer player )
        {

            switch (entityName)
            {
                case "Dead drifter":
                    {
                        AssimCounter += 2 * ZenithSettings.ZAssimCreatureValue ; 
                      
                        break;
                    }

                case "Dead deep drifter":
                    {
                        AssimCounter += 2 * ZenithSettings.ZAssimCreatureValue;
                        break;
                    }

                case "Dead tainted drifter":
                    {
                        AssimCounter += 3 * ZenithSettings.ZAssimCreatureValue * ZenithSettings.ZAssimCreatureValue;
                        break;
                    }

                case "Dead corrupt drifter":
                    {
                        AssimCounter += 4 * ZenithSettings.ZAssimCreatureValue;
                        break;
                    }

                case "Dead nightmare drifter":
                    {
                        AssimCounter += 5 * ZenithSettings.ZAssimCreatureValue;
                        break;
                    }

                case "Dead double-headed drifter":
                    {
                        AssimCounter += 6 * ZenithSettings.ZAssimCreatureValue;
                        break;
                    }

                default:
                    {
                        player.SendIngameError("AssimError", "Unknown Entity!"); 
                        break;
                    }
            }
            var sapi = entity.World.Api as ICoreServerAPI;

            sapi.SendIngameDiscovery(player, $"AssimDisc {entityName}", $"Assimilated {entityName}");
            OnAssimChanged?.Invoke();
            SaveAssim();
            AssimStageUp();
        }





        private void AssimStageUp()
        {
            if (AssimCounter >= ZenithSettings.ZAssimThreshold)
            {
                AssimCounter = 0;
                AssimStage++;
                SaveAssim();
                OnAssimChanged?.Invoke();
                OnAssimStageUp.Invoke();
            }
           
        }


        private void SaveAssim()
        {
            Log("[FLOW] SaveAssim Called");


            watchedZenith.SetInt("AssimCounter", AssimCounter);
            watchedZenith.SetInt("AssimStage", AssimStage);


            entity.WatchedAttributes.MarkPathDirty("zenith");
            Log($"[SAVE] AssimCounter : {watchedZenith.GetInt("AssimCounter", 0)} | AssimStage : {AssimStage}"); 
        }

        public void LoadAssim()
        {
            AssimCounter = watchedZenith.GetInt("AssimCounter", 0);
           AssimStage = watchedZenith.GetInt("AssimStage", 0);
            Log($"[LOAD] AssimCounter : {watchedZenith.GetInt("AssimCounter", 0)} | AssimStage : {AssimStage}");
        }


        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }
      
        // Fix Consumed Attribute - Postponed
        //Add Proper Events for UI - Interchangable with adding UI
        // 
        // 8. AFTER THIS WORKS:
        
        //      - Add progression effects
        // Add UI - Could Use an interface thats connected through Zsystems




        public override string PropertyName()
        {
            return "Assimilate";
        }
    }
}
