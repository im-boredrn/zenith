using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using static zenith.Core.Assimilation.AssimilationCore;
using CreatureType = zenith.Core.Assimilation.AssimilationCore.CreatureType;
namespace zenith.Core.Adaptations
{
    public abstract class Adaptation(IWorldAccessor worldAccessor, Entity entity)
    {
      //  protected readonly IWorldAccessor worldAccessor;
       // protected readonly Entity entity;
        public abstract  CreatureType SourceCreature { get;  }

       


        public virtual void OnAssimilate( Entity entity, CreatureDefinition creatureDefinition,
            IReadOnlyDictionary<CreatureType, CreatureDefinition> creatureDef) { }
      
        public virtual void Apply() { }
        public virtual void OnEvolve( ) { }

        
        public abstract string AdaptationName { get; }
        public abstract string AdaptationDescription { get; }
        public abstract string LockedDescription { get; }
     //   public virtual bool IsUnlocked { get; }
        
    }
}
