using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;

namespace zenith.Core.Helper
{
    public class DomainMapAndEnum
    {

        public enum DomainEnum
        {
            Kinetic,
            Thermal,
            Cold,
            Toxic,
            Bleed,
            Drown,
            None

        }

       public static readonly Dictionary<EnumDamageType, DomainEnum> DamageDomainMap =
    new()
{
    { EnumDamageType.BluntAttack, DomainEnum.Kinetic },
    { EnumDamageType.Gravity, DomainEnum.Kinetic },

    { EnumDamageType.Fire, DomainEnum.Thermal },
    { EnumDamageType.Heat, DomainEnum.Thermal },

    { EnumDamageType.Acid, DomainEnum.Toxic },
    { EnumDamageType.Poison, DomainEnum.Toxic },

    { EnumDamageType.SlashingAttack, DomainEnum.Bleed },
    { EnumDamageType.PiercingAttack, DomainEnum.Bleed },

    {EnumDamageType.Frost, DomainEnum.Cold },

        {EnumDamageType.Suffocation, DomainEnum.Drown }
};

    }
}
