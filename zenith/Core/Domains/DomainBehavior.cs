using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using zenith.Config;
using zenith.Core.Helper;

namespace zenith.Core.Domains
{
    public class DomainBehavior(Entity entity, DomainState domainState)
    {


        static public int Threshold => ZenithSettings.ZGlobalDomainThreshold;
        static public int MaxTier => ZenithSettings.ZGlobalDomainMaxTier;
        public static float DamageReductionPerTier => ZenithSettings.ZDamageReductionPerTier;

        public event Action OnTierUp; // Mixed
        public event Action DomainMaxed; // Mixed
        public event Action OnDomainChanged; // Global - aka No need for params



        public void ProcessDamage(float damage)
        {
            domainState.OldTier = domainState.Tier;

            domainState.Counter += damage;

            if (domainState.Counter >= Threshold && domainState.Tier < MaxTier)
            {
                domainState.Tier++;
                domainState.Counter = 0; 

                if (domainState.Tier > domainState.OldTier) 
                {
                    OnTierUp?.Invoke(); 
                    Logger.Log(entity as EntityPlayer,$"[SERVER] Tier increased to {domainState.Tier}");
                    NotifyChanged();

                }

                
            }

            if (domainState.Tier == MaxTier && !domainState.ContributedToEvolution)
            {
                domainState.ContributedToEvolution = true;
                domainState.IsMaxed = true;
                domainState.Counter = 0;
                DomainMaxed?.Invoke();
                NotifyChanged();

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
        public float Resistance( )
        {


            float resistance = domainState.Tier * DamageReductionPerTier; // each tier adds resistance value
           


            //  Console.WriteLine($"[DEBUG] Damage After resist: {damage}");
            return resistance;

        }





        public void NotifyChanged()
        {
            OnDomainChanged?.Invoke();
            // Also mark zenith dirty so GUI updates can trigger


            entity.WatchedAttributes.MarkPathDirty("zenith");
        }


        public void LoadState(float counter, int tier, bool maxed)
        {
            domainState. Counter = counter;
            domainState.Tier = tier;
            domainState.IsMaxed = maxed;
        }


    }
}
