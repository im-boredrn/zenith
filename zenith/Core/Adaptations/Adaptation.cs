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

        protected Adaptation(IWorldAccessor worldAccessor)
        {
            this.worldAccessor = worldAccessor;
        }

        public virtual void Tick() { }

        public virtual void OnAssimilate(Entity entity, CreatureDefinition creatureDefinition,
            IReadOnlyDictionary<CreatureType, CreatureDefinition> creatureDef) { }


        
    }
}
