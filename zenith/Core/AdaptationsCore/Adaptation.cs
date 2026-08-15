using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using static zenith.Core.Assimilation.AssimilationCore;
using CreatureType = zenith.Core.Adaptations.CreatureDefinition.CreatureType;
using BlockCategory = zenith.Core.AdaptationsCore.BlockDefinitions.BlockCategory;
using zenith.Core.AdaptationsCore;
namespace zenith.Core.Adaptations
{
    public abstract class Adaptation(IWorldAccessor worldAccessor, Entity entity)
    {
      //  protected readonly IWorldAccessor worldAccessor;
       // protected readonly Entity entity;
        public virtual  CreatureType SourceCreature { get;  }
        public virtual BlockCategory BlockCategory { get; }
       public abstract AdaptCategory.AdaptationCategory AdaptationCategory { get; }


        public virtual void OnAssimilate( Entity entity, CreatureDefinition creatureDefinition,
            IReadOnlyDictionary<CreatureType, CreatureDefinition> creatureDef) { }
      
        public virtual void Initialize() { }
        public virtual void StageUp( )
        {
            if (ReadyToEvolve)
            {
                CurrentStage++;
                Initialize();
                EvolutionRequirement *= 2;
                ReadyToEvolve = false;
            }

        }

        
        public abstract string AdaptationName { get; }
        public abstract string AdaptationDescription { get; }
        public abstract string LockedDescription { get; }
        public virtual bool ReadyToEvolve { get; private set; }

        public virtual int EvolutionRequirement { get; private set; }
        public virtual int CurrentStage { get; private set; } = 1;
    }
}
