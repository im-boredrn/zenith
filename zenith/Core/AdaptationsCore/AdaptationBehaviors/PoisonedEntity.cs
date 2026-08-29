using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using zenith.Core.AdaptationsCore.AdaptationData;
using zenith.Core.Helper;
using zenith.Core.NetWork.Packets;

namespace zenith.Core.AdaptationsCore.AdaptationBehaviors
{
    public sealed class PoisonedEntity : EntityBehavior, ITickable, IDisposable
    {
        private readonly EntityAgent EntityAgent;
        private readonly PoisonState PoisonState;
        public bool _disposed;
        private ICoreServerAPI sapi => entity.World.Api as ICoreServerAPI;
        public int PoisonedStack { get; set; } = 0; //state
        private float PoisonDuration = 5;
        public PoisonedEntity(Entity entity, PoisonState poisonState) : base(entity)
        {
            PoisonState = poisonState;
            EntityAgent = entity as EntityAgent;

            TickManager.RegisterClientTick(this);
        }

        public void ApplyPoisonDamage()
        {

            if (!entity.Alive) return;

            var targetHealth = entity.GetBehavior<EntityBehaviorHealth>();

            if (targetHealth != null)
            {

                var zenithNetwork = sapi.ModLoader.GetModSystem<ZenithCore>().ZenithNetwork;

                var packet = new Sounds.SizzleSoundPacket
                {
                    EntityID = entity.EntityId,
                    SoundCode = "sounds/poison/sizzle",
                    PitchMin = 0.8f,
                    PitchMax = 1f
                };



                foreach (IServerPlayer serverPlayer in sapi.World.AllOnlinePlayers)
                {

                    double distanceSq = serverPlayer.Entity.Pos.SquareDistanceTo(entity.Pos);

                    if (distanceSq < 30 * 30)
                        zenithNetwork.ServerChannel.SendPacket<Sounds.SizzleSoundPacket>(packet, serverPlayer);

                }


                targetHealth.Health -= PoisonState.PoisonDamage;
                
            }

            if (targetHealth.Health <= 0)
                entity.Die(EnumDespawnReason.Death, null);

            PoisonedStack--;
        }


        public void OnTick(Entity player, float dt)
        {


            if (!PoisonState.IsUnlocked) return;

            PoisonDuration -= dt;

            if (PoisonDuration <= 0)
            {
                if (PoisonedStack <= 0)
                {
                    this.Dispose();
                    return;
                }

                ApplyPoisonDamage();
            }

        }

      


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

          void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                TickManager.UnRegisterClientTick(this);

            }
            _disposed = true;
        }


        public override string PropertyName()
        {
            return "PoisonedEntity";
        }
    }
}
