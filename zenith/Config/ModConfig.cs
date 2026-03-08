namespace zenith.Config;


/// <summary>
/// Represents the configuration settings for the mod, including parameters that control damage reduction, tier limits,
/// and stage advancement requirements.
/// </summary>
/// <remarks>Use this class to customize gameplay mechanics by adjusting values such as the amount of damage
/// reduction per tier, the maximum achievable tier, the threshold for tier progression, and the requirements for
/// advancing stages. Changing these settings can significantly affect the difficulty and progression within the
/// mod.</remarks>
public class ModConfig
{
    public float DamageReductionPerTier { get; set; } = 0.25f;
    public int GlobalDomainMaxTier { get; set; } = 4;
    public int GlobalDomainThreshold { get; set; } = 100;
    public int StageUpRequirement { get; set; } = 3;
    public bool DebugMode { get; set; } = false;

}