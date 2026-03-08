using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zenith.Config;
using static zenith.Core.ZenithBehavior;

namespace zenith.Core
{
    // Tracks Three Numbers
    //  Stage
    // Domain Points
    // Evolution Points 
    internal class ProgressionManager
    {

        // OnDomain Maxed Domain Points++
        // If DomainPoints == 3
        // Stage ++ 

        private int DomainPoints;
        private int Stage;
        private int StageUpRequirement;
        private int EvolutionPoints;

        Dictionary<DomainEnum, DomainSponge> domains;

        public ProgressionManager()
        {
            ModConfig config = new ModConfig();

            DomainSponge sponge = new DomainSponge(config);

            sponge.DomainMaxed += HandleDomainMaxed;

        }

        private void HandleDomainMaxed(DomainSponge obj)
        {
            DomainPoints++;
            CheckStageProgression();

        }

        private void HandleDomainMaxed(DomainEnum domain)
        {
            var domainSponge = domains[domain];
        }

        private void CheckStageProgression()
        {
            bool stageIncreased = false;

            if (DomainPoints >= StageUpRequirement && Stage < 3)
            {
                Stage++;
                DomainPoints = 0;
                stageIncreased = true;
            }

            if (Stage == 3)
            {
                EvolutionPoints += 20;
            }
          
            if (stageIncreased)
            { OnStageUp?.Invoke(this); }
        }
        public event Action<ProgressionManager> OnStageUp;
    }
}

