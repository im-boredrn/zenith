using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using zenith.Core.Assimilation;
using zenith.Core.Helper;
using zenith.Core.NetWork.Packets;

namespace zenith.Core.NetWork
{
    public class ZenithNetwork
    {
        public IClientNetworkChannel ClientChannel;
        public IServerNetworkChannel ServerChannel;


        public void RegisterServer(ICoreServerAPI sapi)
        {

            ServerChannel = sapi.Network
                .RegisterChannel("zenith")
                .RegisterMessageType<ConsumePacket>()
                .RegisterMessageType<IncreasePacket>()
                .RegisterMessageType<DecreasePacket>()
                .RegisterMessageType<SelectedStatPacket>()
                .RegisterMessageType<OpenAssimInventoryPacket>()
                .RegisterMessageType<CloseAssimInventoryPacket>()
                .RegisterMessageType<SubmitAssimItemPacket>()
                .RegisterMessageType<Sounds.AssimedSoundPacket>()
                .RegisterMessageType<Sounds.AdaptationGainedSoundPacket>()
                .RegisterMessageType<Sounds.EatingSoundPacket>()
                .RegisterMessageType<Sounds.SizzleSoundPacket>()


                .SetMessageHandler<ConsumePacket>((player, packet) =>
                {
                    var behavior = player.Entity.GetBehavior<ZenithBehaviorServer>();
                    behavior?.ServerSystems?.AssimilationCore?.TryAssimilate(player);

                    Logger.Log(player.Entity, "[DEBUG] ConsumePacket Received!");
                })
                .SetMessageHandler<IncreasePacket>((player, packet) =>
                {

                    var behavior = player.Entity.GetBehavior<ZenithBehaviorServer>();

                    behavior?.ServerSystems?.StatOutput?.OutputChange(packet.ShiftHeld,packet.AltHeld, "Increase");
                })
                .SetMessageHandler<DecreasePacket>((player,packet) =>
                {

                    var behavior = player.Entity.GetBehavior<ZenithBehaviorServer>();


                    behavior?.ServerSystems?.StatOutput?.OutputChange(packet.ShiftHeld, packet.AltHeld, "Decrease");
                })
                .SetMessageHandler<SelectedStatPacket>((player,packet) =>
                {
                    var behavior = player.Entity.GetBehavior<ZenithBehaviorServer>();
                    behavior?.ServerSystems?.StatOutput?.StatSwitch((StatOutput.StatType)packet.SelectedStat);
                })
                .SetMessageHandler<OpenAssimInventoryPacket>((player,packet) =>
                {
                    var behavior = player.Entity.GetBehavior<ZenithBehaviorServer>();
                    behavior?.ServerSystems.OpenAssimInv(player);
                })
                 .SetMessageHandler<CloseAssimInventoryPacket>((player, packet) =>
                 {
                     var behavior = player.Entity.GetBehavior<ZenithBehaviorServer>();
                     behavior?.ServerSystems.CloseAssimInv(player);
                 })
                  .SetMessageHandler<SubmitAssimItemPacket>((player, packet) =>
                  {
                      var behavior = player.Entity.GetBehavior<ZenithBehaviorServer>();
                      behavior?.ServerSystems.SubmitItemAssim();
                  });

        }

        public void RegisterClient(ICoreClientAPI capi)
        {
            ClientChannel = capi.Network
                .RegisterChannel("zenith")
                .RegisterMessageType<ConsumePacket>()
                .RegisterMessageType<IncreasePacket>()
                .RegisterMessageType<DecreasePacket>()
                .RegisterMessageType<SelectedStatPacket>()
                .RegisterMessageType<OpenAssimInventoryPacket>()
                .RegisterMessageType<CloseAssimInventoryPacket>()
                .RegisterMessageType<SubmitAssimItemPacket>()
                .RegisterMessageType<Sounds.AssimedSoundPacket>()
                .RegisterMessageType<Sounds.AdaptationGainedSoundPacket>()
                .RegisterMessageType<Sounds.EatingSoundPacket>()
                .RegisterMessageType<Sounds.SizzleSoundPacket>()


                .SetMessageHandler<Sounds.AssimedSoundPacket>((packet) =>
                {
                    ZenithCore.PlayPlayerSound(capi, packet.SoundCode, packet.EntityID, packet.PitchMin, packet.PitchMax);
                })

                .SetMessageHandler<Sounds.AdaptationGainedSoundPacket>((packet) =>
                {
                    ZenithCore.PlayPlayerSound(capi, packet.SoundCode, packet.EntityID, packet.PitchMin, packet.PitchMax);
                })

                .SetMessageHandler<Sounds.EatingSoundPacket>((packet) =>
                {
                    ZenithCore.PlayPlayerSound(capi, packet.SoundCode, packet.EntityID, packet.PitchMin, packet.PitchMax);
                })

                .SetMessageHandler<Sounds.SizzleSoundPacket>((packet) =>
                {
                    ZenithCore.PlayPlayerSound(capi, packet.SoundCode, packet.EntityID, packet.PitchMin, packet.PitchMax);
                });


        }

        public void RequestStat(StatOutput.StatType stat)
        {
            ClientChannel?.SendPacket(new SelectedStatPacket()
            {
                SelectedStat = (int)stat
            });
             
        }

        public void RequestOpenAssimInventory()
        {
            ClientChannel?.SendPacket(new OpenAssimInventoryPacket()
            {
               // Maybe put in toggleMode one day
            });
        }
        public void RequestCloseAssimInventory()
        {
            ClientChannel?.SendPacket(new CloseAssimInventoryPacket()
            {
            });
        }
        public void RequestSubmitItem()
        {
            ClientChannel?.SendPacket(new SubmitAssimItemPacket());


        }

        public void Request(GlKeys glKeys, bool shift, bool alt)
        {

            switch (glKeys)
            {
               

                case GlKeys.V:
                    {
                        ClientChannel?.SendPacket(new ConsumePacket());
                        break;
                    }

                case GlKeys.B :
                    {
                        ClientChannel?.SendPacket(new IncreasePacket()
                        {
                            ShiftHeld = shift,
                            AltHeld = alt

                        });
                        break;
                    }

                case GlKeys.N:
                    {
                        ClientChannel?.SendPacket(new DecreasePacket()
                        {
                            ShiftHeld = shift,
                            AltHeld = alt
                        });
                        break;
                    }
            }
        }
    }
}
