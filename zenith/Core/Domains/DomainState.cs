using System;
using System.Collections.Generic;
using System.Text;

namespace zenith.Core.Domains
{
    public class DomainState
    {
        public bool ContributedToEvolution { get;  set; } = false; 

        public float Counter { get; set; } = 0; 
        public int Tier { get; set; } = 0;
        public int OldTier { get; set; } 
        public bool IsMaxed { get; set; } = false; 

    }
}
