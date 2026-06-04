using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zenith.Core.Domains
{
     public interface IDomainInfo
    {
        int GetTier();
        bool IsDMaxed();

        float GetResistanceValue();

        DomainSponge GetDomain()


        
       

    }
}
