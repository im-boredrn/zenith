using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using static zenith.Core.Assimilation.AssimilationCore;
using CreatureType = zenith.Core.Definitions.CreatureDefinition.CreatureType;
using BlockCategory = zenith.Core.Definitions.BlockDefinitions.BlockCategory;
using zenith.Core.AdaptationsCore;
namespace zenith.Core.Definitions
{
    public abstract class AdaptationDefinitions(IWorldAccessor worldAccessor, Entity entity)
    {
      //  protected readonly IWorldAccessor worldAccessor;
       // protected readonly Entity entity;
        public virtual  CreatureType SourceCreature { get; init; }
        public virtual BlockCategory BlockCategory { get; init; }
       public abstract AdaptCategory.AdaptationCategory AdaptationCategory { get; init; }


        public virtual void OnAssimilate( Entity entity, CreatureDefinition creatureDefinition,
            IReadOnlyDictionary<CreatureType, CreatureDefinition> creatureDef) { } //Behavior

       

        
        public abstract string AdaptationName { get; init; }
        public abstract string AdaptationDescription { get; init; }
        public abstract string LockedDescription { get; init; } 
        public virtual string BlockCode { get; init; }
    }
}
