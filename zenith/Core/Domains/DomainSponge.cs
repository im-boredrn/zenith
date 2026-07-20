using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using zenith.Config;
using static zenith.Core.ZenithBehavior;

namespace zenith.Core.Domains
{
    //Domains store only:

//    Domain
//        Tier
//    Experience

//They should not store stage logic.
// In other words Domain only knows about domain
    public class DomainSponge : IDomainInfo // Dont Forget to make Properties Public
    {
        static private ModConfig Config => ConfigLoader.Config; // May be useless
        public bool ContributedToEvolution { get; private set; } = false;
        static public int Threshold => ZenithSettings.ZGlobalDomainThreshold;
        static public int MaxTier => ZenithSettings.ZGlobalDomainMaxTier;
        public float DamageReductionPerTier => ZenithSettings.ZDamageReductionPerTier;
        static public bool DebugMode => ZenithSettings.ZDebugMode;

        private readonly ModConfig config;
        public DomainEnum Domain { get; }

        private readonly Entity entity;

        public float Counter = 0;
        public int Tier;
        public int OldTier;
        public bool IsMaxed = false;
        public string DomainName => Domain.ToString();
      //  private int MemoryLevel;



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

       

        public event Action<IDomainInfo> OnTierUp; // Mixed
        public event Action DomainMaxed; // Mixed
        public event Action OnDomainChanged; // Global - aka No need for params



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
                Log($"[SERVER] Tier increased to {Tier}");
                    OnDomainChanged?.Invoke();
                    NotifyChanged();

                }

                if (Tier == MaxTier && !ContributedToEvolution)
                {
                    ContributedToEvolution = true;
                    IsMaxed = true;
                    Counter = 0;
                    DomainMaxed?.Invoke();
                    OnDomainChanged?.Invoke();
                    NotifyChanged();

                }
            }
          
        }


     

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

        


       
        public void NotifyChanged()
        {
            OnDomainChanged?.Invoke();
            // Also mark zenith dirty so GUI updates can trigger
            entity.WatchedAttributes.MarkPathDirty("zenith");
        }

        public DomainEnum GetDomain()
        {
            return Domain;
        }

        public string GetDomainName()
        {
            return DomainName;
        }
        public bool IsDMaxed()
        {
            return IsMaxed;
        }

        public int GetTier()
        {
            return Tier;
        }

        public int GetMaxTier()
        {
            return MaxTier;
        }

        public float GetCounter()
        {
            return Counter;
        }

        public int GetThreshold()
        {
            return Threshold;
        }
        public float GetResistanceValue()
        {
            return Tier * DamageReductionPerTier;
        }

        public void LoadState(float counter, int tier, bool maxed)
        {
            Counter = counter;
            Tier = tier;
            IsMaxed = maxed;
        }

        private void Log(string message)
        {
            if (!DebugMode) return;
            entity.World.Logger.Warning(message);
        }
    }
}
