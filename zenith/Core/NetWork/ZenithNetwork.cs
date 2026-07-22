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
                    Handle(player, "Consume"); // Once packet is requested this launches
                })
                .SetMessageHandler<SwitchPacket>((player, packet) =>
                {
                    Handle(player, "Switch");
                })
                .SetMessageHandler<IncreasePacket>((player, packet) =>
                {
                    Handle(player, "Increase");
                }
            )
                .SetMessageHandler<DecreasePacket>((player,packet) =>
                {
                    Handle(player, "Decrease");
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

       
        public void Request(GlKeys glKeys)
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

                case GlKeys.AltLeft:
                    {
                        ClientChannel?.SendPacket(new IncreasePacket());
                        break;
                    }

                case GlKeys.AltRight:
                    {
                        ClientChannel?.SendPacket(new DecreasePacket());
                        break;
                    }
            }
        }

        private void Handle (IServerPlayer player, string packet)
        {
            var behavior = player.Entity.GetBehavior<ZenithBehavior>();

            switch (packet)
            {
                case "Consume":
                    {
                        behavior?.systems?.AssimilationCore?.TryAssimilate(player);


                        break;
                    }

                case "Switch":
                    {
                        behavior?.systems.StatOutput?.StatSwitch();
                        break;
                    }

                case "Increase":
                    {
                        behavior?.systems?.StatOutput?.OutputChange("Increase"); // Could also pass down an int
                        break;
                    }

                case "Decrease":
                    {
                        behavior?.systems?.StatOutput?.OutputChange("Decrease"); 
                        break;
                    }
            }
        } 
    }
}
