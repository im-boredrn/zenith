using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using zenith.Config;

namespace zenith.Core.Helper
{
    static class Logger
    {

        public static bool DebugMode => ZenithSettings.ZDebugMode;

        public static void Log(EntityPlayer entityPlayer, string message)
        {
            if (DebugMode)
            entityPlayer.World.Logger.Warning(message);
        }
    }
}
