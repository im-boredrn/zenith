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
namespace zenith.Core.Adaptations
{
    public class WolfAdaptation : Adaptation // Corpse Consumption
    {
        private readonly Entity Entity;
        private readonly IWorldAccessor world;
        EntityPlayer Player => Entity as EntityPlayer;


        public WolfAdaptation(IWorldAccessor world, Entity entity) : base(world,entity)
        {
            this.Entity = entity;
            this.world = world;
        }

        public override void OnAssimilate(Entity entity , CreatureDefinition creatureDefinition,
            IReadOnlyDictionary<CreatureType, CreatureDefinition> creatureDef)
        {



            float Sat;
            Sat = creatureDefinition.NutritionVal * 100f;

            var player = entity as EntityPlayer;
            
            player.ReceiveSaturation(Sat, EnumFoodCategory.Protein, 10f, 2f);
        }

        public override CreatureType SourceCreature => CreatureType.wolf;

    }

    public class BearAdaptation : Adaptation
    {

    private readonly IWorldAccessor world;
        private readonly EntityPlayer Player;
        static public bool DebugMode => ZenithSettings.ZDebugMode;


        public BearAdaptation(IWorldAccessor world, EntityPlayer entity) : base(world, entity) // Bear Sense, Pack Mule
        {
            this.Player = entity;
            this.world = world;
        }

        public override void Tick(float dt)
        {
            
            if ( Player == null) return;
            var sapi = entity.World.Api as ICoreServerAPI;



            var nearest = world.GetNearestEntity(Player.Pos.XYZ, 30, 20);

            var entities = world.GetEntitiesAround(Player.Pos.XYZ, 30, 20);

            foreach (var nearbyEntity in entities)
            {
                double distance = nearbyEntity.Pos.DistanceTo(Player.Pos.XYZ);

                sapi.SendMessage(Player.Player, GlobalConstants.AllChatGroups,
                    $"{nearbyEntity.Code} - {distance:F1}m ", EnumChatType.Notification);
            //    Log($"{entity.Code} - {distance:F1}m");

            }

        }

        public override CreatureType SourceCreature => CreatureType.bear;



                 private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }

    }


}

   

