using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace zenith.Core.Helper
{
    public interface ITickable
    {
      public void OnTick( Entity entity,float dt);

    }
}
