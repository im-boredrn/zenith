using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using zenith.Config;
using static zenith.Core.ZenithBehavior;

namespace zenith.Core
{
    //Domains store only:

//    Domain
//        Tier
//    Experience

//They should not store stage logic.
// In other words Domain only knows about domain
    public class DomainSponge // Dont Forget to make Properties Public
    {
        private ModConfig Config => ConfigLoader.Config; // points to the static config
        public bool ContributedToEvolution { get; private set; } = false;
        public int Threshold => ZenithSettings.ZGlobalDomainThreshold;
        public int MaxTier => ZenithSettings.ZGlobalDomainMaxTier;
        public float DamageReductionPerTier => ZenithSettings.ZDamageReductionPerTier;
        public bool DebugMode => ZenithSettings.ZDebugMode;

        private readonly ModConfig config;
        public DomainEnum Domain { get; }

        private readonly Entity entity;

        public float Counter = 0;
        public int Tier = 0;
        public int OldTier;
        private int MemoryLevel;



        /// <summary>
        /// Initializes a new instance of the DomainSponge class using the specified configuration and entity.
        /// </summary>
        /// <remarks>Each DomainSponge instance maintains its own configuration and entity, enabling
        /// independent state management.</remarks>
        /// <param name="config">The configuration settings to apply to this DomainSponge instance.</param>
        /// <param name="entity">The entity associated with this DomainSponge, used for saving and loading state.</param>

        private EntityPlayer Player => entity as EntityPlayer;
        public DomainSponge(ModConfig config, Entity entity, DomainEnum domain)
        {
            this.config = config;
            this.Domain = domain;
            this.entity = entity;
        }

       

        public event Action<DomainSponge> OnTierUp;
        public event Action<DomainSponge> DomainMaxed;



        /// <summary>
        /// Processes the specified amount of damage and updates the tier if the accumulated damage meets or exceeds the
        /// threshold.
        /// </summary>
        /// <remarks>If the accumulated damage exceeds the threshold and the current tier is below the
        /// maximum, the tier is incremented and the counter is reset. Events are triggered to notify subscribers when
        /// the tier is upgraded or when the maximum tier is reached.</remarks>
        /// <param name="damage">The amount of damage to process. Must be a non-negative value; influences tier progression based on the
        /// current counter and threshold.</param>
        public void ProcessDamage(float damage)
        {
            OldTier = Tier;

            Counter += damage;

            if (Counter >= Threshold && Tier < MaxTier)// not <=MaxTier because if the tier is 2 it still fulfils requirements so it can go up one more
            {
                Tier++;
                Counter = 0; // Reset

              if (Tier > OldTier ) // Publisher of Event Should Never Care if its registered on the subscriber(ZenithBehavior)
                {
                    OnTierUp?.Invoke(this); // Fires To EVERYONE, Subscriber Decides to Receive

                }

                if (Tier == MaxTier && !ContributedToEvolution)
                {
                    ContributedToEvolution = true;
                    DomainMaxed?.Invoke(this);
                }
            }
          
        }

        /// <summary>
        /// Handles the event when a player's tier is increased and logs the new tier level.
        /// </summary>
        /// <remarks>This method is triggered when a player's tier is upgraded. It is recommended to
        /// ensure that the player is properly initialized before invoking this method. The method sends a notification
        /// message to the player indicating the new tier level.</remarks>
        


 


            /// <summary>
            /// Calculates the final damage value after applying resistance based on the character's tier and damage
            /// reduction per tier.
            /// </summary>
            /// <remarks>Resistance is determined by multiplying the character's tier by the damage
            /// reduction per tier. Higher tiers provide greater resistance, reducing incoming damage more
            /// effectively.</remarks>
            /// <param name="damage">The initial damage amount to be reduced by resistance. Must be a non-negative value.</param>
            /// <returns>The resulting damage value after resistance is applied. The value is clamped to a minimum of zero.</returns>
        public float Resistance( float damage)
        {


            float resistance = Tier * DamageReductionPerTier; // each tier adds resistance value
            float reducedDamage;
          // Console.WriteLine($"[DEBUG] Damage before resist: {damage}");

            reducedDamage = damage / (1f + resistance);
            damage = Math.Max(0, reducedDamage); // clamp

          //  Console.WriteLine($"[DEBUG] Damage After resist: {damage}");
            return damage;

        }

        public float GetResistanceValue()
        {
            return Tier * DamageReductionPerTier;
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
