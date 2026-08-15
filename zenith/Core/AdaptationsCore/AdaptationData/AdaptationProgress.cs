using System;
using System.Collections.Generic;
using System.Text;
using zenith.Core.Definitions;

namespace zenith.Core.AdaptationsCore.AdaptationData
{
    public class AdaptationProgress
    {
        public int Counter { get; set; }
        public int BlockLVL { get; set; }
        public bool IsUnlocked { get; set; }
        public int EvolutionStage { get; set; } = 1;
        public int EvolutionRequirement { get; set; }
        public static Dictionary<string, AdaptationProgress> CreateProgress()
        {
            var progress = new Dictionary<string, AdaptationProgress>();

            foreach (var creature in CreatureDefinition.CreatureLibrary.Keys) // give each creature progression
                progress[creature.ToString()] = new AdaptationProgress();

            foreach (var block in BlockDefinitions.BlockLibrary.Keys)
                progress[block] = new AdaptationProgress(); // give each block progression

            return progress;
        }
    }
}
