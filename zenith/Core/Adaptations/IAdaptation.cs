using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common.Entities;
using CreatureType = zenith.Core.Assimilation.AssimilationCore.CreatureType;
namespace zenith.Core.Adaptations
{
    public interface IAdaptation
    {
      public  CreatureType SourceCreature { get; }

        void ApplyAdaptation(Entity entity);

        void OnAssimilate(Entity entity, CreatureDefinition creatureDefinition, IReadOnlyDictionary<CreatureType, CreatureDefinition> creatureDef );

    }
}
