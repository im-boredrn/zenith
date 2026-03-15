using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core.Abilities;
using zenith.Core.Domains;
using static zenith.Core.ZenithBehavior;

namespace zenith.Core
{
  
    // Tracks Three Numbers
    //  Stage
    // Domain Points
    // Evolution Points 
    public class ProgressionManager
    {

        
        private bool DebugMode => ZenithSettings.ZDebugMode;
        public int DomainPoints { get; private set; } // Dont Add values Ruins Persistance(set defaults in watched Attributes
        public int Stage { get; private set; }
        private int StageUpRequirement => ZenithSettings.ZStageUpRequirement ;
        private int EvolutionPoints;

        private TreeAttribute watchedZenith;

        private readonly Entity entity;
        private EntityPlayer Player => entity as EntityPlayer;
        // Constructor
        public ProgressionManager(Entity entity )
        {
            this.entity = entity;
            watchedZenith = (TreeAttribute)(entity.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entity.WatchedAttributes["zenith"] = watchedZenith;
        }


        /// <summary>
        /// Handles the event when the domain reaches its maximum capacity by incrementing domain points and evaluating
        /// stage progression.
        /// </summary>
        /// <remarks>This method should be called when the domain is fully saturated. It updates the
        /// domain points and checks whether the stage progression criteria are met. Ensure that the domain state is
        /// valid before invoking this method.</remarks>
        /// <param name="sponge">The DomainSponge instance representing the current domain state that triggered the event. Must not be null.</param>
        public void HandleDomainMaxed(DomainSponge sponge)
        {
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
                Log($"[DATA] DomainPoints : {DomainPoints}");
            }
            else
            {
                EvolutionPoints += 20;
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
            bool stageIncreased = false;

            if (DomainPoints >= StageUpRequirement && Stage < 3)
            {
                Stage++;
                DomainPoints = 0;
                stageIncreased = true;
                if (stageIncreased) 
                {
                    ApplyPassivesForAllDomains();
                    OnStageUpSave();
                    OnStageUp?.Invoke(this);

                }

            }
        }
        public event Action<ProgressionManager> OnStageUp;
        public float GetStageMultiplier()
        {
            if (Stage == 1)
            {
                return ZenithSettings.ZStageResistanceMultiplier1;
            }
            else if (Stage == 2) return ZenithSettings.ZStageResistanceMultiplier2;
            else if (Stage == 3) return ZenithSettings.ZStageResistanceMultiplier3;

            return 1;
        }

        public void ApplyPassives(DomainEnum domain)
        {

            if (!CanUseAbilities())
            {
                Log($"[DATA] Current Stage is : {Stage} | Returning...");
                return; 
            }



            var passive = abilityFactory.CreatePassives(domain);
            if(passive != null)
            passive.Apply(Player);

            Log($"[DATA] {domain} passive applied!"); ;
        }

        public void TickPassives()
        {
            if (!CanUseAbilities())
            {
                Log($"[DATA] Current Stage is : {Stage} | Returning...");
                return;
            }
            foreach (var domain in Enum.GetValues<DomainEnum>().Where(d => d != DomainEnum.None))
            {
                var passive = abilityFactory.CreatePassives(domain);
                passive?.Tick(Player);
            }
        }


        public void HandleAttack(DomainEnum domain, DamageSource source, EntityAgent target)
        {
            if (!CanUseAbilities()) 
            {
                Log($"[DATA] Current Stage is : {Stage} | Returning...");
                return;
            }

            var attack = abilityFactory.CreateAttack(domain);

            if(attack != null)
            attack.OnAttack(source, target);
        }

        private AbilityFactory abilityFactory;

        public void SetFactory(AbilityFactory factory)
        {
            this.abilityFactory = factory;
        }



        /// <summary>
        /// Handles the event when the player's stage increases by notifying the player and saving their progression.
        /// </summary>
        /// <remarks>This method sends a notification message to the player indicating the new stage and
        /// logs the event. The player must be connected to the server API for the notification to be sent.</remarks>
        private void OnStageUpSave()
        {
            Log($"[EVENT] stage increased to {Stage}");
            var sapi = entity.World.Api as ICoreServerAPI;
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
            watchedZenith.SetInt("Stage", Stage);
            watchedZenith.SetInt("DomainPoints", DomainPoints);
            watchedZenith.SetInt("EvolutionPoints", EvolutionPoints);

            entity.WatchedAttributes.MarkPathDirty("zenith");

            //string keyStage = "zenith." + entity.GetName() + ".Stage";
            //    string keyDPoints = "zenith." + entity.GetName() + ".DomainPoints";
            //    string keyEPoints = "zenith." + entity.GetName() + ".EvolutionPoints";

            //    entity.WatchedAttributes.SetInt(keyStage, Stage);
            //    entity.WatchedAttributes.SetInt(keyDPoints, DomainPoints);
            //    entity.WatchedAttributes.SetInt(keyEPoints, EvolutionPoints);

            
            //    entity.WatchedAttributes.MarkPathDirty(keyStage);
            //    entity.WatchedAttributes.MarkPathDirty(keyDPoints);
            //    entity.WatchedAttributes.MarkPathDirty(keyEPoints);
            
        }

        public void LoadProgression()
        {
            watchedZenith = (TreeAttribute)(entity.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entity.WatchedAttributes["zenith"] = watchedZenith;

            Stage = watchedZenith.GetInt("Stage", 1);
            DomainPoints = watchedZenith.GetInt("DomainPoints", 0);
            EvolutionPoints = watchedZenith.GetInt("EvolutionPoints", 0);


            //string keyStage = "zenith." + entity.GetName() + ".Stage";
            //string keyDPoints = "zenith." + entity.GetName() + ".DomainPoints";
            //string keyEPoints = "zenith." + entity.GetName() + ".EvolutionPoints";

            //// Defaults if nothing was saved yet
            //Stage = entity.WatchedAttributes.GetInt(keyStage, 1);
            //DomainPoints = entity.WatchedAttributes.GetInt(keyDPoints, 0);
            //EvolutionPoints = entity.WatchedAttributes.GetInt(keyEPoints, 0);

            Log($"[LOAD] Stage: {Stage}, DomainPoints: {DomainPoints}, EvolutionPoints: {EvolutionPoints}");
        }

        private void ApplyPassivesForAllDomains()
        {
            foreach (DomainEnum domain in Enum.GetValues<DomainEnum>())
            {
                if (domain == DomainEnum.None) continue;
                ApplyPassives(domain);
            }
        }

        public void ApplyAttackAbilitiesForAllDomains(DamageSource source, EntityAgent targetEntity)
        {
            
            foreach (DomainEnum domain in Enum.GetValues<DomainEnum>())
            {
                if (domain == DomainEnum.None) continue;


                HandleAttack(domain, source, targetEntity );
            }
        }
        private bool CanUseAbilities()
        {
            if (Stage < 2)
            {
                Log($"[DATA] Current Stage is : {Stage} | Returning...");
                return false;
            }

            return true;
        }
        public string GetStageName(int stage)
        {

            return stage switch
            {
                1 => "Adapting Organism",
                2 => "Hyper-Adaptive Organism",
                3 => "Paragon of Seraphs",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// Logs a warning message to the world logger if debug mode is enabled.
        /// </summary>
        /// <remarks>This method is intended for use during development and debugging. It will not log
        /// messages if debug mode is disabled, ensuring that only relevant information is captured during debugging
        /// sessions.</remarks>
        /// <param name="message">The message to log as a warning. This should provide context about the event or condition being logged.</param>
        private void Log(string message)
        {
            if (!DebugMode) return;
            entity.World.Logger.Warning(message);
        }
    }
}

