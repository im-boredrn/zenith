using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using zenith.Config;

namespace zenith.Core
{
    public class DomainSponge // Dont Forget to make Properties Public
    {
        private ModConfig Config => ConfigLoader.Config; // points to the static config
        public int Threshold => ZenithSettings.ZGlobalDomainThreshold;
        public int MaxTier => ZenithSettings.ZGlobalDomainMaxTier;
        public float DamageReductionPerTier => ZenithSettings.ZDamageReductionPerTier;


        private readonly ModConfig config;
        public float Counter = 0;
        public int Tier = 0;
        public int OldTier;
        public int DomainCompletion;
        public int DomainPoints { get; private set; } = 0; // awarded at stage 3
        public int Stage { get; private set; } = 1; // current stage
        public int StageUpRequirement => 3; // number of Domain completions needed to stage up
        public bool TierEventRegistered { get; set; } = false;

        //  public int Resistance = 0; out until I find a good way to incorparate

        public DomainSponge(ModConfig config)
        {
            this.config = config;
        }

       

        public event Action<DomainSponge> OnTierUp;
        public event Action<DomainSponge> OnStageUp;


        public void ProcessDamage(float damage)
        {
            OldTier = Tier;

            Counter += damage;

            if (Counter >= Threshold && Tier < MaxTier)// not <=MaxTier because if the tier is 2 it still fulfils requirements so it can go up one more
            {
                Tier++;
                Counter = 0; // Reset

              if (Tier > OldTier && !TierEventRegistered)
                {
                    OnTierUp?.Invoke(this);
                    TierEventRegistered = true;
                }
                TierEventRegistered = false;
                if (Tier == MaxTier)
                    CheckStageProgression();
              
            }
          
        }

        public void CheckStageProgression()
        {
            DomainCompletion++;

            if (DomainCompletion >= StageUpRequirement && Stage < 3 )
            {
                Stage++;
                DomainCompletion = 0;
            }

            if (Stage == 3)
            {
                DomainPoints += 20;
            }
            OnStageUp.Invoke(this);
        }


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




        //public void TierReactions()
        //{
        //    switch (Tier)
        //    {
        //        case 1:
        //            {
        //                sapi.SendMessage(Player.Player, GlobalConstants.AllChatGroups, $"{domain} Raised | Tier is Now {tier}! \n " +
        //                    $"{domain} damage hurts you less", EnumChatType.Notification);
        //                break;
        //            }
        //        case 2:
        //            {
        //                sapi.SendMessage(Player.Player, GlobalConstants.AllChatGroups, $"{domain} Raised | Tier is Now {tier}! \n " +
        //                  $"Your body is more resilient to {domain} damage", EnumChatType.Notification);
        //                break;
        //            }

        //        case 3:
        //            {
        //                sapi.SendMessage(Player.Player, GlobalConstants.AllChatGroups, $"{domain} Raised | Tier is Now {tier}! \n " +
        //               $"Your body has adapted to {domain} damage", EnumChatType.Notification);
        //                break;
        //            }
        //    }

        // }

    }
}
