using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
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
                .RegisterMessageType<SwitchPacket>()
                .RegisterMessageType<IncreasePacket>()
                .RegisterMessageType<DecreasePacket>()


                .SetMessageHandler<ConsumePacket>((player, packet) =>
                {
                    var behavior = player.Entity.GetBehavior<ZenithBehavior>();
                    behavior?.systems?.AssimilationCore?.TryAssimilate(player);
                })
                .SetMessageHandler<SwitchPacket>((player, packet) =>
                {
                    var behavior = player.Entity.GetBehavior<ZenithBehavior>();
                    behavior?.systems.StatOutput?.StatSwitch();

                })
                .SetMessageHandler<IncreasePacket>((player, packet) =>
                {

                    var behavior = player.Entity.GetBehavior<ZenithBehavior>();

                    behavior.systems.StatOutput.OutputChange(packet.ShiftHeld,packet.AltHeld, "Increase");
                })
                .SetMessageHandler<DecreasePacket>((player,packet) =>
                {

                    var behavior = player.Entity.GetBehavior<ZenithBehavior>();


                    behavior?.systems?.StatOutput?.OutputChange(packet.ShiftHeld, packet.AltHeld, "Decrease");
                });

        }

        public void RegisterClient(ICoreClientAPI capi)
        {
            ClientChannel = capi.Network
                .RegisterChannel("zenith")
                .RegisterMessageType<ConsumePacket>()
                .RegisterMessageType<SwitchPacket>()
                .RegisterMessageType<IncreasePacket>()
                .RegisterMessageType<DecreasePacket>();
        }

       
        public void Request(GlKeys glKeys, bool shift, bool alt)
        {

            switch (glKeys)
            {
                case GlKeys.Keypad9:
                    {
                        ClientChannel?.SendPacket(new SwitchPacket());
                        break;
                    }

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
