using Cairo;
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
using zenith.Core.Assimilation;
using zenith.Core.Adaptations;
using zenith.Core.Domains;
using zenith.Core.NetWork;
using zenith.GUI;
namespace zenith;
public class ZenithCore : ModSystem
{
    private ICoreServerAPI sapi;

    public static ILogger Logger { get; private set; }
    public static string ModId { get; private set; }
    public static ICoreAPI Api { get; private set; }
    public ZenithGui ZenithGui;

    public ZenithNetwork ZenithNetwork { get; private set; }
    public static ModConfig Config => ConfigLoader.Config;
 //   private readonly long tickListenerId;   

    public override void StartPre(ICoreAPI api)
    {
        base.StartPre(api);
        Api = api;
        Logger = Mod.Logger;
        ModId = Mod.Info.ModID;
    }
    
    public override void Start(ICoreAPI api)
    {
        base.Start(api);

        ZenithNetwork = new ZenithNetwork();

        
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
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

            }, "AttachBehaviors");
        };

        ZenithNetwork.RegisterServer(sapi);
    }

    public override void StartClientSide(ICoreClientAPI api) 
    {
#pragma warning disable IDE0019

        ZenithNetwork.RegisterClient(api);


       
        
            api.Event.PlayerJoin += (IClientPlayer player) =>
            { // Apparently Causes Crash, Might not be My mod though | Could be null player.
                api.Event.EnqueueMainThreadTask(() =>
                {
                    try
                    {
                        api.Logger.Notification("AttachBehaviors task started");

                        if (player.Entity == null)
                        {
                            api.Logger.Error("Player entity was null!");
                            return;
                        }

                        if (!player.Entity.HasBehavior<ZenithBehavior>())
                        {
                            // Pass capi here so GUI can be initialized
                            player.Entity.AddBehavior(new ZenithBehavior(player.Entity));
                            api.Logger.Notification($"Zenith Behavior attached to {player.PlayerName}");
                        }

                    }
                    catch (Exception e)
                    {
                        Logger.Error($"Start Client Side EnqueueMainThreadTask Threw {e}");
                    }

                   


                }, "AttachBehaviors");

            };

        
       


        Keybinds.WireKeybinds(api,ZenithNetwork);

     
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

        api.Input.RegisterHotKey("togglesense", "Toggle Bear Senses", GlKeys.I, HotkeyType.GUIOrOtherControls);
        api.Input.SetHotKeyHandler("togglesense", comb =>
        {
            var player = api.World.Player?.Entity as EntityPlayer;
            if (player == null) return false;

            var behavior = player.GetBehavior<ZenithBehavior>();
            if (behavior?.systems?.BearSenseRenderer != null)
            {
                behavior.systems.BearSenseRenderer.ToggleBearSense();
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
                zenithBehavior.systems.OnServerTick(dt); 
            }
        }
    }

    public override void Dispose()
    {
        Logger = null;
        ModId = null;
        Api = null;
        base.Dispose();

     
    }
}
