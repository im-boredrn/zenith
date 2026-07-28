using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using CreatureType = zenith.Core.Assimilation.AssimilationCore.CreatureType;
namespace zenith.Core.Assimilation.Adaptations
{
    public class WolfAdaptation : IAdaptation
    {
        private readonly Entity entity;

        EntityPlayer Player => entity as EntityPlayer;
      public  CreatureType SourceCreature { get; }
        public void ApplyAdaptation(Entity entity)
        {

        }

        public void OnAssimilate(Entity entity)
        {
            float sat;
            sat = 
            Player.ReceiveSaturation(sat, EnumFoodCategory.Protein, 10f, 2f);
        }
    }

    public class BearAdaptation : IAdaptation
    {
        public CreatureType SourceCreature { get; }
        public void ApplyAdaptation(Entity entity)
        {

        }

        public void OnAssimilate(Entity entity)
        {

        }
    }
}
