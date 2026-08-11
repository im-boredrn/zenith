using System;
using System.Collections.Generic;
using System.Text;

namespace zenith.Core.Helper
{
    public static class TickManager
    {

        public static readonly  List<ITickable> ClientTick = [];
        public static readonly List<ITickable> ServerTick = [];

        public static void RegisterClientTick(ITickable clientTickable)
        {
            ClientTick.Add(clientTickable);
        }

        public static void RegisterServerTick(ITickable serverTick)
        {
            ServerTick.Add(serverTick);
        }

    }
}
