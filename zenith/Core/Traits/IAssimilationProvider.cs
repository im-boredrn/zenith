using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using zenith.Config;
using static zenith.Core.Assimilation.Assimilation;

namespace zenith.Core.Traits
{
    public interface IAssimilationProvider
    {
        int GetCreatureLevel(CreatureType creatureType);

    }
}
