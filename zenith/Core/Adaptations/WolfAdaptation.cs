using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using zenith.Config;
using zenith.Core.Adaptations;
using static zenith.Core.Assimilation.AssimilationCore;
using CreatureType = zenith.Core.Assimilation.AssimilationCore.CreatureType;
namespace zenith.Core.Adaptations
{
    public class WolfAdaptation : Adaptation // Corpse Consumption
    {
        private readonly Entity entity;
        private readonly IWorldAccessor world;
        EntityPlayer Player => entity as EntityPlayer;
      public  CreatureType SourceCreature { get; }


        public WolfAdaptation(IWorldAccessor world) : base(world)
        {
            this.world = world;
        }

        public override void OnAssimilate(Entity entity , CreatureDefinition creatureDefinition,
            IReadOnlyDictionary<CreatureType, CreatureDefinition> creatureDef)
        {

            if (!creatureDef[CreatureType.wolf].AdaptAchieved) return;


            float Sat;
            Sat = creatureDefinition.NutritionVal * 100f;

            var player = entity as EntityPlayer;
            
            player.ReceiveSaturation(Sat, EnumFoodCategory.Protein, 10f, 2f);
        }

    }

    public class BearAdaptation : Adaptation
    {
        private readonly Entity entity;

        private readonly ICoreAPI coreAPI;
    private readonly IWorldAccessor world;
        static public bool DebugMode => ZenithSettings.ZDebugMode;


        EntityPlayer Player => entity as EntityPlayer;
        // public CreatureType SourceCreature { get; }


        public BearAdaptation(IWorldAccessor world) : base(world)
        {
            this.world = world;
        }
        public  void GetDistance(ICoreClientAPI capi)
        {
            Vec3d direction = entity.Pos.XYZ - Player.Pos.XYZ;

            double distance = direction.Length();
            var entities = capi.World.GetNearestEntity(direction, 30, 20);

            var en = capi.World.GetEntitiesAround(Player.Pos.XYZ, 30, 20);

            Log($"{en}, Nearest Entity: {entities}");
        }


                 private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }

    }


}

   

