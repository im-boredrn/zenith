using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core.Domains;
using static zenith.Core.ZenithBehaviorServer;

namespace zenith.Core.Progression
{
  
    // Tracks Three Numbers
    //  Stage
    // Domain Points
    // Evolution Points 
    public class ProgressionManager : IStageProvider
    {


        private static bool DebugMode => ZenithSettings.ZDebugMode;
        public int DomainPoints { get; private set; } // Dont Add values Ruins Persistance(set defaults in watched Attributes
        public int Stage { get; private set; }
        public string StageName { get; private set; }
       static private int StageUpRequirement => ZenithSettings.ZStageUpRequirement ;


        private readonly Entity entity;
        private readonly ZenithData data;
        private EntityPlayer Player => entity as EntityPlayer;
        // Constructor
        public ProgressionManager(Entity entity, ZenithData zenithData )
        {
            this.entity = entity;
            this.data = zenithData;

        }


        public event Action OnProgressionChanged; // UI only
        public event Action OnStageUp; // UI only 

        /// <summary>
        /// Handles the event when the domain reaches its maximum capacity by incrementing domain points and evaluating
        /// stage progression.
        /// </summary>
        /// <remarks>This method should be called when the domain is fully saturated. It updates the
        /// domain points and checks whether the stage progression criteria are met. Ensure that the domain state is
        /// valid before invoking this method.</remarks>
        /// <param name="sponge">The DomainSponge instance representing the current domain state that triggered the event. Must not be null.</param>
        public void HandleDomainMaxed()
        {
#pragma warning disable IDE0019

            Log("[FLOW] HandleDomainMaxed Called");
            var sapi = entity.World.Api as ICoreServerAPI;
            if (sapi == null)
            {
                Log("[DATA] Sapi is Null returning...");
                return;

            }
          
            if (Stage < 3)
            {
                DomainPoints++;
                SaveProgression();
                Log($"[DATA] DomainPoints : {DomainPoints}");
            }
            else
            {
                SaveProgression();
            }

            CheckStageProgression();

        }

     
        /// <summary>
        /// Checks the current stage progression and advances the stage if the accumulated domain points meet the
        /// requirement.
        /// </summary>
        /// <remarks>If the stage is increased, domain points are reset and a save operation is triggered.
        /// Upon reaching stage 3, additional evolution points are awarded. This method should be called after updating
        /// domain points to ensure progression is handled correctly.</remarks>
        private void CheckStageProgression()
        {
            bool stageIncreased ;

            if (DomainPoints >= StageUpRequirement && Stage < 3)
            {
                Stage++;
                DomainPoints = 0;
                stageIncreased = true;
                if (stageIncreased) 
                {
                    OnStageUpSave();
                    OnStageUp?.Invoke();

                }

            }
        }
        public float GetStageMultiplier()
        {
            return Stage switch
            {
                1 => ZenithSettings.ZStageMultiplier1,
                2 => ZenithSettings.ZStageMultiplier2,
                3 => ZenithSettings.ZStageMultiplier3,

                _ => 0f
            };
        }

        public float GetResistanceMultiplier()
        {
            return Stage switch
            {
                1 => ZenithSettings.ZStageResistanceMultiplier1,
                2 => ZenithSettings.ZStageResistanceMultiplier2,
                3 => ZenithSettings.ZStageResistanceMultiplier3,
                _ => 0f

            };
        }

 
        public float GetMiningSpeedMultiplier()
        {
            return Stage switch
            {
                2 => ZenithSettings.ZStageMiningSpeedMultipiler2,
                3 => ZenithSettings.ZStageMiningSpeedMultipiler3,
                _ => 0f
            };

        }



        public float GetIgniteChanceMultiplier()
        {
            return Stage switch
            {
                2 => ZenithSettings.ZStageIgniteChanceMultipiler2,
                3 => ZenithSettings.ZStageIgniteChanceMultipiler3,
                _ => 0f
            };
        }

        


        /// <summary>
        /// Handles the event when the player's stage increases by notifying the player and saving their progression.
        /// </summary>
        /// <remarks>This method sends a notification message to the player indicating the new stage and
        /// logs the event. The player must be connected to the server API for the notification to be sent.</remarks>
        private void OnStageUpSave()
        {
#pragma warning disable IDE0019

            Log($"[EVENT] stage increased to {Stage}");
            var sapi = entity.World.Api as ICoreServerAPI;
#pragma warning restore IDE0019

            if (sapi == null) return;
            var player = Player.Player;


            sapi.SendMessage( player,
                GlobalConstants.AllChatGroups,
                $"Stage increased!! New Stage: {Stage}",
                EnumChatType.Notification
                );
            SaveProgression();
        }



        /// <summary>
        /// Saves the player's current progression state, including stage, domain points, and evolution points, to
        /// persistent storage.
        /// </summary>
        /// <remarks>This method updates the player's watched attributes for stage, domain points, and
        /// evolution points, and marks these attributes as modified. Call this method to ensure that the player's
        /// progression is recorded and can be restored in future sessions.</remarks>
        private void SaveProgression()
        {
            Log("[FLOW] SaveProgression Called");
            var dataTree = data.Tree;
            dataTree.SetInt("Stage", Stage);
            dataTree.SetInt("DomainPoints", DomainPoints);
            dataTree.SetString("StageName", GetStageName());
            dataTree.SetFloat("StageMult", GetStageMultiplier());
        

            entity.WatchedAttributes.MarkPathDirty("zenith");

            OnProgressionChanged?.Invoke();
           Log("[EVENT] OnProgressionChanged fired");

        }

        public void LoadProgression()
        {
            var dataTree = data.Tree;


            Stage = dataTree.GetInt("Stage", 1);
            DomainPoints = dataTree.GetInt("DomainPoints", 0);
             StageName = dataTree.GetString("StageName", GetStageName());

            Log($"[LOAD] Stage: {Stage} | Stage Name: {StageName} | DomainPoints: {DomainPoints}");
        }

     


        public int GetStage()
        {
            return Stage;

        }

        public string GetStageName()
        {
            
            return Stage switch
            {
                1 => "Adapting Organism",
                2 => "Hyper-Adaptive Organism",
                3 => "Paragon",
                _ => "Unknown"
            };
        }

      
        private void Log(string message)
        {
            if (!DebugMode) return;
            entity.World.Logger.Warning(message);
        }
    }
}

