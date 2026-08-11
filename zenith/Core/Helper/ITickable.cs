using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;

namespace zenith.Core.Helper
{
    public interface ITickable
    {
      public void OnTick( EntityPlayer entityPlayer,float dt);
    }
}
