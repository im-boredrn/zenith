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
using zenith.GUI;

namespace zenith;
public class zenithCore : ModSystem
{
    private ICoreServerAPI sapi;

    public static ILogger Logger { get; private set; }
    public static string ModId { get; private set; }
    public static ICoreAPI Api { get; private set; }
    public ZenithGui ZenithGui;
    public static Harmony HarmonyInstance { get; private set; }
    public static ModConfig Config => ConfigLoader.Config;
 //   private readonly long tickListenerId;
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
        // Always fix VintageStory Console Warnings before debugging your code. Invisible Entities was not the fault of Zenith
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

        // tickListenerId = api.Event.RegisterGameTickListener(OnServerTick, 1000);
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        
        api.Event.PlayerJoin += (IClientPlayer player) =>
        {
            api.Event.EnqueueMainThreadTask(() =>
            {
                if (!player.Entity.HasBehavior<ZenithBehavior>())
                {
                    // Pass capi here so GUI can be initialized
                    player.Entity.AddBehavior(new ZenithBehavior(player.Entity));
                    api.Logger.Notification($"Zenith Behavior attached to {player.PlayerName}");
                }
            }, "AttachZenithBehavior");

        };

        // Register hotkey once, at client start
        api.Input.RegisterHotKey("opendomain", "Open Organism GUI", GlKeys.G, HotkeyType.GUIOrOtherControls);
        api.Input.SetHotKeyHandler("opendomain", comb =>
        {
            var player = api.World.Player?.Entity as EntityPlayer;
            if (player == null) return false;

            var behavior = player.GetBehavior<ZenithBehavior>();
            if (behavior?.systems?.ZenithGui != null)
            {
                behavior.systems.ZenithGui.Toggle();
                return true;
            }

            return false;
        });
    }


    
    

    private void OnServerTick(float dt)
    {

        foreach (var player in sapi.World.AllOnlinePlayers)
        {
            var entityPlayer = player.Entity as EntityPlayer;
            if (entityPlayer == null) continue;

            var zenithBehavior = entityPlayer.GetBehavior<ZenithBehavior>();
            if (zenithBehavior?.systems != null)
            {
                zenithBehavior.systems.AbilityFactory.TickPassives();
            }
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

       // if (tickListenerId > 0)
        {
      //      sapi.Event.UnregisterGameTickListener(tickListenerId);
        }
    }
}
