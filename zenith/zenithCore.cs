using Cairo;
using HarmonyLib;
using System;
using System.Numerics;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core;
using zenith.Core.Abilities;

namespace zenith;
public class zenithCore : ModSystem
{
    private ICoreServerAPI sapi;

    public static ILogger Logger { get; private set; }
    public static string ModId { get; private set; }
    public static ICoreAPI Api { get; private set; }
    public static Harmony HarmonyInstance { get; private set; }
    public static ModConfig Config => ConfigLoader.Config;
    private long tickListenerId;
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
        // Always fix warnings before debugging your code. Invisible Entities was not the fault of Zenith
        sapi = api;
        api.Event.PlayerNowPlaying += (IServerPlayer player) =>
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

        tickListenerId = api.Event.RegisterGameTickListener(OnServerTick, 1000);
    }

    private void OnServerTick(float dt)
    {

            foreach (IServerPlayer player in sapi?.World.AllOnlinePlayers ?? Array.Empty<IServerPlayer>())
            {
                var entity = player?.Entity as EntityPlayer;
                if (entity == null || entity.World == null) continue; // skip if null       


           // TODO Find a way to implement this without creating a new object here systems.TickPassives();
        }

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
