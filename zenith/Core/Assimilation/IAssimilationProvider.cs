using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using zenith.Config;
using static zenith.Core.Assimilation.AssimilationCore;

namespace zenith.Core.Assimilation
{
    public interface IAssimilationProvider
    {
        float GetCreatureLevel(CreatureType creatureType);
        public TraitTotals CalculateTotals();

    }
}
