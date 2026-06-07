using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static zenith.Core.ZenithBehavior;

namespace zenith.Core.Domains
{
     public interface IDomainInfo
    {
        int GetTier();
        float GetCounter();

        int GetThreshold();
        bool IsDMaxed();

        float GetResistanceValue();

        void LoadState(float counter, int tier, bool maxed);

        void ProcessDamage(float damage);

        DomainEnum GetDomain();
        string GetDomainName();

        event Action<IDomainInfo> OnTierUp;
        event Action DomainMaxed;
        event Action OnDomainChanged;


    }
}
