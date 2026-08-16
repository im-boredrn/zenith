using System;
using System.Collections.Generic;
using System.Text;
using zenith.Core.Definitions;
using static zenith.Core.Definitions.CreatureDefinition;

namespace zenith.Core.AdaptationsCore.AdaptationData
{
    public abstract class AdaptationState 
    {
        public  virtual int Counter { get; set; }
        public virtual int BlockLVL { get; set; }
        public virtual bool IsUnlocked { get; set; }
        public virtual int EvolutionStage { get; set; } = 1;
        public virtual int EvolutionRequirement { get; set; }

        
    }
}
