using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using static zenith.Core.Assimilation.AssimilationCore;

namespace zenith.Core.Adaptations
{
    public abstract class Adaptation
    {
        protected readonly IWorldAccessor worldAccessor;
        protected readonly Entity entity;
        protected Adaptation(IWorldAccessor worldAccessor, Entity entity)
        {
            this.entity = entity;
            this.worldAccessor = worldAccessor;
        }

        public virtual void Tick(float deltaTime) { }

        public virtual void OnAssimilate(Entity entity, CreatureDefinition creatureDefinition,
            IReadOnlyDictionary<CreatureType, CreatureDefinition> creatureDef) { }


        
    }
}
