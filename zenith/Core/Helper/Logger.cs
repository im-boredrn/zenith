using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;

namespace zenith.Core.Helper
{
    static class Logger
    {


        public static void Log(EntityPlayer entityPlayer, string message)
        {
            entityPlayer.World.Logger.Warning(message);
        }
    }
}
