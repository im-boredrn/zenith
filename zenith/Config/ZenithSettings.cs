using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zenith.Config
{
    internal class ZenithSettings
    {
        public static ModConfig Config => ConfigLoader.Config; // points to loaded configs
        public static float ZDamageReductionPerTier => Config?.DamageReductionPerTier ?? 0.25f;
        public static int ZGlobalDomainMaxTier => Config?.GlobalDomainMaxTier ?? 4;
        public static int ZGlobalDomainThreshold => Config?.GlobalDomainThreshold ?? 100;

    }
}
