using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using static zenith.Core.Assimilation.AssimilationCore;
using CreatureType = zenith.Core.Assimilation.AssimilationCore.CreatureType;
namespace zenith.Core.Adaptations
{
    public abstract class Adaptation
    {
        protected readonly IWorldAccessor worldAccessor;
        protected readonly Entity entity;
        public abstract  CreatureType SourceCreature { get;  }

        protected Adaptation(IWorldAccessor worldAccessor, Entity entity)
        {
            this.entity = entity;
            this.worldAccessor = worldAccessor;
        }


        public virtual void OnAssimilate(Entity entity, CreatureDefinition creatureDefinition,
            IReadOnlyDictionary<CreatureType, CreatureDefinition> creatureDef) { }


        
    }
}
