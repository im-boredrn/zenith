using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace zenith.Core.Abilities
{
    public interface IPassives
    {
        public void Apply(EntityPlayer entityPlayer);
        void Tick(EntityPlayer entityPlayer);  // called every server tick
    }

    public interface IAttackAbilities
    {
        void OnAttack(DamageSource source, EntityAgent targetEntity );
    }

    public interface IActiveAbilites
    {
        void Activate();
    }
}
