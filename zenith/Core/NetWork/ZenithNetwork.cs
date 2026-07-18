using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using zenith.Core.Assimilation;

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
                .SetMessageHandler<ConsumePacket>((player, packet) =>
                {
                    HandleAssimilation(player); // Once packet is requested this launches
                });

        }

        public void RegisterClient(ICoreClientAPI capi)
        {
            ClientChannel = capi.Network
                .RegisterChannel("zenith")
                .RegisterMessageType<ConsumePacket>();
        }

        public void RequestAssimilation() // keybind
        {
            ClientChannel?.SendPacket(new ConsumePacket());
        }



        private void HandleAssimilation(IServerPlayer player) //Execute
        {
            var behavior = player.Entity.GetBehavior<ZenithBehavior>();
            behavior?.systems?.Assimilation?.TryAssimilate(player);
        }
    }
}
