using System;
using System.Collections.Generic;
using System.Text;
using zenith.Core.Definitions;
using static zenith.Core.Definitions.CreatureDefinition;

namespace zenith.Core.AdaptationsCore.AdaptationData
{
    public class AdaptationState // instanced
    {
        public int Counter { get; set; }
        public int BlockLVL { get; set; }
        public bool IsUnlocked { get; set; }
        public int EvolutionStage { get; set; } = 1;
        public int EvolutionRequirement { get; set; }
        public static Dictionary<Type, AdaptationState> CreateProgress()
        {
            var progress = new Dictionary<Type, AdaptationState>();

            foreach (var creature in CreatureDefinition.CreatureLibrary.Values) // give each creature progression
            {
                if (creature.AdaptationType != null)
                {
                    progress[creature.AdaptationType] = new AdaptationState();
                }
            }

            foreach (var block in BlockDefinitions.BlockLibrary.Values)
            {
                if (block.AdaptationType != null)
                {
                    progress[block.AdaptationType] = new AdaptationState();
                }
            }
            return progress;
        }
    }
}
