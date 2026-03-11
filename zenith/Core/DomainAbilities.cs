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

        public void Apply( EntityPlayer entityPlayer) 
        {
            entityPlayer.Properties.KnockbackResistance *= 2f;
            entityPlayer.Properties.FallDamageMultiplier *= 0.5f;
        }
    }

    internal class ThermalAbilities : IAbilitiesManager
    {
        public void Apply(EntityPlayer entityPlayer)
        {

        }
    }
    internal class ToxicAbilities : IAbilitiesManager
    {
        public void Apply(EntityPlayer entityPlayer)
        {

        }
    }
    internal class ColdAbilities : IAbilitiesManager
    {
        public void Apply(EntityPlayer entityPlayer)
        {

        }
    }
    internal class HemorrhageAbilities : IAbilitiesManager
    {
        public void Apply(EntityPlayer entityPlayer)
        {

        }
    }
}
