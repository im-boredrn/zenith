using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Server;

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
                .SetMessageHandler<ConsumePacket>((player, packet) =>
                {
                    HandleAssimilation(player); // Once packet is requested this launches
                })
                .SetMessageHandler<SwitchPacket>((player, packet) =>
                {
                    HandleSwitch(player);
                });

        }

        public void RegisterClient(ICoreClientAPI capi)
        {
            ClientChannel = capi.Network
                .RegisterChannel("zenith")
                .RegisterMessageType<ConsumePacket>()
                .RegisterMessageType<SwitchPacket>();
        }

        public void RequestAssimilation() // keybind
        {
            ClientChannel?.SendPacket(new ConsumePacket());
        }
        public void RequestSwitch()
        {
            ClientChannel?.SendPacket(new SwitchPacket());
        }

        private void HandleSwitch (IServerPlayer player)
        {
            var behavior = player.Entity.GetBehavior<ZenithBehavior>();

            behavior?.systems.StatOutput?.StatSwitch();
        }

        private void HandleAssimilation(IServerPlayer player) //Execute
        {
            var behavior = player.Entity.GetBehavior<ZenithBehavior>();
            behavior?.systems?.AssimilationCore?.TryAssimilate(player);
        }
    }
}
