using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zenith.Core;

namespace zenith.Config
{
    /// <summary>
    /// Provides access to configuration settings for the Zenith system, including damage reduction, domain thresholds,
    /// and stage requirements.
    /// </summary>
    /// <remarks>This class exposes static properties that retrieve values from the loaded configuration. If
    /// the configuration is not available, default values are used. Intended for internal use within the
    /// application.</remarks>
    internal class ZenithSettings
    {
        public static ModConfig Config => ConfigLoader.Config; // points to loaded configs
        public static float ZDamageReductionPerTier => Config?.DamageReductionPerTier ?? 0.25f;
        public static int ZGlobalDomainMaxTier => Config?.GlobalDomainMaxTier ?? 4;
        public static int ZGlobalDomainThreshold => Config?.GlobalDomainThreshold ?? 100;
        public static int ZStageUpRequirement => Config?.StageUpRequirement ?? 3;
        public static float ZStageResistanceMultiplier1 => Config?.StageResistanceMultiplier1 ?? 1.0f;
        public static float ZStageResistanceMultiplier2 => Config?.StageResistanceMultiplier2 ?? 1.2f;
        public static float ZStageResistanceMultiplier3 => Config?.StageResistanceMultiplier3 ?? 1.4f;
        public static bool ZDebugMode => Config?.GlobalDebugMode ?? false;
        public static float ZRegenAmount => Config?.RegenAmount ?? 0.25f;


        
        
    }
}
