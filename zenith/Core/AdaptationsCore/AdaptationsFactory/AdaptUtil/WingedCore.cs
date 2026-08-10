//using System;
//using System.Collections.Generic;
//using System.Text;
//using Vintagestory.API.Client;
//using Vintagestory.API.Common;
//using Vintagestory.API.Common.Entities;
//using Vintagestory.API.MathTools;

//namespace zenith.Core.AdaptationsCore.AdaptationsFactory.AdaptUtil
//{
//    public class WingedCore : ModSystem
//    {
//        private ICoreClientAPI capi;
//        private bool patched = false;

//        public override void StartClientSide(ICoreClientAPI api)
//        {
//            base.StartClientSide(api);


//            capi = api;

//            RegisterPhysicsPatch();

//            api.Input.RegisterHotKey("togglewings", "Toggle Wings", GlKeys.K, HotkeyType.GUIOrOtherControls);
//            api.Input.SetHotKeyHandler("togglewings", comb =>
//            {
//                var player = api.World.Player?.Entity as EntityPlayer;
//                if (player == null) return false;

//                var behavior = player.GetBehavior<ZenithBehavior>();
//                if (behavior?.systems?.WingedEnabler != null)
//                {
//                    behavior.systems.WingedEnabler.ToggleWings();
//                    capi.Logger.Notification($"[WINGED DATA] HasWings? {behavior.systems.WingedEnabler.HasWings}");
//                    return true;
//                }
//                return false;
//            });
//        }


//        private void RegisterPhysicsPatch()
//        {
//            if (capi == null) return;



//            capi.Event.PlayerJoin += OnPlayerJoin;

//            if (capi.World.Player?.Entity != null) ApplyPatch(capi.World.Player.Entity);

//            capi.Event.RegisterGameTickListener(dt =>
//            {
//                if (!patched && capi.World.Player.Entity != null)
//                {
//                    ApplyPatch(capi.World.Player.Entity);
//                }

//            },80,100);


//        }

//        private void OnPlayerJoin(IClientPlayer byPlayer)
//        {
//            if (byPlayer.Entity != null)
//                ApplyPatch(byPlayer.Entity);
//        }

//        private void ApplyPatch(Entity entity)
//        {
//            if (patched || entity == null) return;

//            capi?.Logger?.Notification("[PATCH] Applying Winged Patch");
//            WingedPhysicsPatch.PatchWingedPhysics(entity);
//            patched = true;
//         }


//        public void ApplyWingedPhysics(float dt, Entity entity, EntityPos pos, EntityControls controls)
//        {


//            if (entity.GetBehavior<ZenithBehavior>().systems?.WingedEnabler == null) return;


//                if (!controls.Gliding || !entity.WatchedAttributes.HasAttribute("winged") || !entity.GetBehavior<ZenithBehavior>().systems.WingedEnabler.HasWings) return;
//                double cosPitch = Math.Cos(pos.Pitch);
//                double num = Math.Sin(pos.Pitch);
//                double cosYaw = Math.Cos(pos.Yaw);
//                double sinYaw = Math.Sin(pos.Yaw);
//                double glideFactor = num + 0.15;
//                controls.GlideSpeed = GameMath.Clamp(controls.GlideSpeed - glideFactor * (double)dt * 0.25, 0.004999999888241291, 0.75);
//                double glideSpeed = GameMath.Clamp(max: (double)entity.Stats.GetBlended("gliderSpeedMax") - 0.8, val: controls.GlideSpeed, min: 0.004999999888241291);
//                float gliderLiftMax = entity.Stats.GetBlended("gliderLiftMax");
//                double pitch = Math.Min(num * glideSpeed, gliderLiftMax);

//                pos.Motion.Add((-cosPitch) * sinYaw * glideSpeed, pitch, (-cosPitch) * cosYaw * glideSpeed);

//                double drag = GameMath.Clamp(1f - pos.Motion.Length() * 0.3f, 0f, 1f);
//                pos.Motion.Mul(drag);

            
//        }
//    }
//}
