using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace zenith.Core
{
  

    internal class KineticAbilities : IAbilitiesManager
    {

        public void Apply( EntityProperties entityProperties)
        {
            entityProperties.KnockbackResistance *= 22f;
        }
    }

    internal class ThermalAbilities : IAbilitiesManager
    {
        public void Apply(EntityProperties entityProperties)
        {

        }
    }
    internal class ToxicAbilities : IAbilitiesManager
    {
        public void Apply(EntityProperties entityProperties)
        {

        }
    }
    internal class ColdAbilities : IAbilitiesManager
    {
        public void Apply(EntityProperties entityProperties)
        {

        }
    }
    internal class HemorrhageAbilities : IAbilitiesManager
    {
        public void Apply(EntityProperties entityProperties)
        {

        }
    }
}
