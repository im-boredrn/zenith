using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common.Entities;
using static zenith.Core.Assimilation.AssimilationCore;

namespace zenith.Core.Adaptations
{
    public abstract class Adaptation
    {

        public virtual void Tick() { }

        public virtual void OnAssimilate(Entity entity, CreatureDefinition creatureDefinition,
            IReadOnlyDictionary<CreatureType, CreatureDefinition> creatureDef) { }
    }
}
