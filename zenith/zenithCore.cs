using zenith.Config;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using zenith.Core;

namespace zenith;
public class zenithCore : ModSystem
{
    private ICoreServerAPI sapi;

    public static ILogger Logger { get; private set; }
    public static string ModId { get; private set; }
    public static ICoreAPI Api { get; private set; }
    public static Harmony HarmonyInstance { get; private set; }
    public static ModConfig Config => ConfigLoader.Config;

    public override void StartPre(ICoreAPI api)
    {
        base.StartPre(api);
        Api = api;
        Logger = Mod.Logger;
        ModId = Mod.Info.ModID;
        HarmonyInstance = new Harmony(ModId);
        HarmonyInstance.PatchAll();
    }
    
    public override void Start(ICoreAPI api)
    {
        base.Start(api);
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        sapi = api;
        api.Event.PlayerJoin += (IServerPlayer player) =>
        {
            sapi.Event.EnqueueMainThreadTask(() =>
            {
                if (!player.Entity.HasBehavior<ZenithBehavior>())
                {
                    player.Entity.AddBehavior(new ZenithBehavior(player.Entity));
                    sapi.Logger.Notification($"Zenith Behavior attached to {player.PlayerName}");
                }
            }, "AttachZenithBehavior");
        };


    }
        
    

    public override void Dispose()
    {
        HarmonyInstance?.UnpatchAll(ModId);
        HarmonyInstance = null;
        Logger = null;
        ModId = null;
        Api = null;
        base.Dispose();
    }
}
