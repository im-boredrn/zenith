using System;
using System.Collections.Generic;
using System.Text;

namespace zenith.Core.Domains
{
    public class Domain
    {

        public DomainState DomainState { get; }


       public DomainBehavior DomainBehavior { get; }


        public Domain(DomainState domainState, DomainBehavior domainBehavior)
        {
            DomainState = domainState;
            DomainBehavior = domainBehavior;
        }
    }
}
