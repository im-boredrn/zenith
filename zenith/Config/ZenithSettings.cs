using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
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
        
        public static float ZStageMultiplier1 => Config?.StageMultiplier1 ?? 1.0f;
        public static float ZStageMultiplier2 => Config?.StageMultiplier2 ?? 1.2f;
        public static float ZStageMultiplier3 => Config?.StageMultiplier3 ?? 1.4f;

        public static float ZStageResistanceMultiplier1 => Config?.StageResistanceMultiplier1 ?? 1.0f;
        public static float ZStageResistanceMultiplier2 => Config?.StageResistanceMultiplier2 ?? 1.2f;
        public static float ZStageResistanceMultiplier3 => Config?.StageResistanceMultiplier3 ?? 1.4f;
        
        public static bool ZDebugMode => Config?.GlobalDebugMode ?? false;
        
        public static float ZRegenAmount => Config?.RegenAmount ?? 0.25f;
        
        public static float ZStageSpeed2 => Config?.StageSpeed2 ?? 0.2f;
        public static float ZStageSpeed3 => Config?.StageSpeed3 ?? 0.3f;
        
        public static float ZStageDamage2 => Config?.StageDamage2 ?? 0.5f;
        public static float ZStageDamage3 => Config?.StageDamage3 ?? 1f;

        public static float ZStageIgniteChanceMultipiler2 => Config?.StageIgniteChanceMultipiler2 ?? 0.1f; // 25%
        public static float ZStageIgniteChanceMultipiler3 => Config?.StageIgniteChanceMultipiler3 ?? 0.2f; // 35%

        public static float ZStageJumpHeightMultipiler => Config?.StageJumpHeightMultipiler ?? 0.2f;

        public static float ZStageMiningSpeedMultipiler2 => Config?.StageMiningSpeedMultipiler2 ?? 2.5f;
        public static float ZStageMiningSpeedMultipiler3 => Config?.StageMiningSpeedMultipiler3 ?? 3.2f;

        //  public static float ZStageArmorWSAMultipiler2 => Config?.StageArmorWSAMultipiler2 ?? -0.2f;
        //  public static float ZStageArmorWSAMultipiler3 => Config?.StageArmorWSAMultipiler3 ?? -0.4f;


        //Assimilation
        public static int ZAssimThreshold => Config?.AssimThreshhold ?? 20;
        public static int ZAssimMaxStage => Config?.AssimMaxStage ?? 5;

        public static int ZAssimCreatureValue => Config?.AssimCreatureValue ?? 1;





    }
}
