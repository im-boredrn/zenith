using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using zenith.Config;
using zenith.Core.Adaptations;
using static zenith.Core.Assimilation.AssimilationCore;
using CreatureType = zenith.Core.Assimilation.AssimilationCore.CreatureType;
namespace zenith.Core.AdaptationsCore.AdaptationsFactory
{
    public class WolfAdaptation(IWorldAccessor world, Entity entity, IReadOnlyDictionary<CreatureType, CreatureDefinition> statReq) : Adaptation(world,entity) 
    {

        EntityPlayer Player => entity as EntityPlayer;


       

        public override void OnAssimilate(Entity entity , CreatureDefinition creatureDefinition,
            IReadOnlyDictionary<CreatureType, CreatureDefinition> creatureDef)
        {



            float Sat;
            Sat = creatureDefinition.NutritionVal * 100f;

            var player = entity as EntityPlayer;
            
            player.ReceiveSaturation(Sat, EnumFoodCategory.Protein, 10f, 2f);
        }

     

        public override CreatureType SourceCreature => CreatureType.wolf;
        public override string AdaptationName => "Corpse Consumption";
        public override string AdaptationDescription => "You've gained the ability to digest assimilated creatures";
        public override string LockedDescription => $"Assimilate {statReq[CreatureType.wolf].Counter}/{statReq[CreatureType.wolf].Threshold} Wolves to unlock ";
        // Locked Description string Later.
    }


}

   

