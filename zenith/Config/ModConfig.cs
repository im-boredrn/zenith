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
    public int GlobalDomainThreshold { get; set; } = 30;
    public int StageUpRequirement { get; set; } = 3;
    public bool GlobalDebugMode { get; set; } = false;

    public float StageMultiplier1 { get; set; } = 1.0f;
    public float StageMultiplier2 { get; set; } = 1.2f;
    public float StageMultiplier3 { get; set; } = 1.4f;

    public float StageResistanceMultiplier1 { get; set; } = 1.0f;
    public float StageResistanceMultiplier2 { get; set; } = 1.2f;
    public float StageResistanceMultiplier3 { get; set; } = 1.4f;

    public float RegenAmount { get; set; } = 0.25f;

    public float StageIgniteChanceMultipiler2 { get; set; } = 0.05f; // 15% Chance
    public float StageIgniteChanceMultipiler3 { get; set; } = 0.1f; // 20% chance

    public float StageMiningSpeedMultipiler2 { get; set; } = 2.5f;
    public float StageMiningSpeedMultipiler3 { get; set; } = 3.2f;

    public int AssimCreatureLVLMult { get; set; } = 1;
    public float InitialJump { get; set; } = 0.3f;

    public int DrifterCreatureMaxLVL { get; set; } = 10;
    public int BowtornCreatureMaxLVL { get; set; } = 10;
    public int ShiverCreatureMaxLVL { get; set; } = 10;

    public int BearCreatureMaxLVL { get; set; } = 10;
    public int HareCreatureMaxLVL { get; set; } = 20;
    public int WolfCreatureMaxLVL { get; set; } = 20;

    public int FoxCreatureMaxLVL { get; set; } = 30;
    public int GoatCreatureMaxLVL { get; set; } = 10;
    public int DeerCreatureMaxLVL { get; set; } = 10;

    public int HyenaCreatureMaxLVL { get; set; } = 10;
    public int PigCreatureMaxLVL { get; set; } = 15;
    public int ChickenCreatureMaxLVL { get; set; } = 15;
    public int SheepCreatureMaxLVL { get; set; } = 10;
    public int RaccoonCreatureMaxLVL { get; set; } = 15;



}