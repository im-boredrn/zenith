using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace zenith.Core
{
    public class DomainSponge // Dont Forget to make Properties Public
    {
        public float Counter = 0;
        public int Threshold = 100;
        public int Tier = 0;
        public int MaxTier = 3;
        public int OldTier;
        public bool TierEventRegistered { get; set; } = false;

        //  public int Resistance = 0; out until I find a good way to incorparate

        public event Action<DomainSponge> OnTierUp;



        public void ProcessDamage(float damage)
        {
            OldTier = Tier;

            Counter += damage;

            if (Counter >= Threshold && Tier < MaxTier)// not <=MaxTier because if the tier is 2 it still fulfils requirements so it can go up one more
            {
                Tier++;
                Counter = 0; // Reset

              if (Tier > OldTier)  OnTierUp?.Invoke(this);
              
            }
          
        }



        public float Resistance(ref float damage)
        {
            float resistance = Tier * 0.25f; // each tier adds resistance value
            float reducedDamage;
           Console.WriteLine($"[DEBUG] Damage before resist: {damage}");

            reducedDamage = damage / (1f + resistance);
            damage = Math.Max(0, reducedDamage); // clamp

            Console.WriteLine($"[DEBUG] Damage After resist: {damage}");
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
