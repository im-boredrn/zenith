using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace zenith.Core
{
    public interface IAbilitiesManager
    {
        public void Apply(EntityPlayer entityPlayer);
    }
}
