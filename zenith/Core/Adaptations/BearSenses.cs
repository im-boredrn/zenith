using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using zenith.Config;
using static zenith.Core.Assimilation.AssimilationCore;

namespace zenith.Core.Adaptations
{
    public class BearSenses : Adaptation
    {
        private readonly IWorldAccessor world;
        private readonly EntityPlayer Player;
        static public bool DebugMode => ZenithSettings.ZDebugMode;


        public BearSenses(IWorldAccessor world, EntityPlayer entity) : base(world, entity) // Bear Sense, Pack Mule
        {
            this.Player = entity;
            this.world = world;
        }

        private float SenseTimer;
        private float DebugTimer;
        private List<SensedEntity> SensedEntities = new();

      


        public override void Tick(float dt)
        {

            SenseTimer += dt;

            if (SenseTimer > 2f)
            {
                UpdateSense();
                SenseTimer = 0;
            }

            if (DebugTimer > 4f)
            {
                DebugSense();
                DebugTimer = 0;
            }
        }

        private void UpdateSense()
        {
            var entities = world.GetEntitiesAround(Player.Pos.XYZ, 30, 20);

            var lookDir = Player.Pos.GetViewVector().ToVec3d();
            


            SensedEntities.Clear();

            foreach (var entity in entities)
            {

                if ($"{entity.Code}".Contains("player")) continue;


                double distance = entity.Pos.DistanceTo(Player.Pos.XYZ);
                Vec3d relative = entity.Pos.XYZ - Player.Pos.XYZ;
                Vec3d direction = relative.Clone().Normalize();
                double dot = direction.Dot(lookDir);

                SensedEntities.Add(new SensedEntity
                {
                    Entity = entity,
                    Direction = direction,
                    Distance = distance,
                    Dot = dot
                    
                });
            }
        }

        private void DebugSense()
        {


         //   if (Player == null) return;
         //   var sapi = entity.World.Api as ICoreServerAPI;
            StringBuilder senseOutput = new StringBuilder();

            foreach (var sensed in SensedEntities)
            {
                senseOutput.AppendLine($"{sensed.Entity.Code} {sensed.Direction} Dot : {GetDirection(sensed.Dot):F2}");

            }
            Log(senseOutput.ToString());
            //sapi.SendMessage(Player.Player, GlobalConstants.AllChatGroups,
            //     senseOutput.ToString(), EnumChatType.Notification);
        }

        public class SensedEntity
        {

            public Entity Entity;
            public Vec3d Direction;

            public double Distance;
            public double Dot;
        }
  string GetDirection(double dot)
        {
            if (dot > 0.5f)
            {
                return ("[BS] IN FRONT");
            }
            else if (dot < -0.5f)
            {
                return ("[BS] BEHIND");
            }
            else
            {
                return ("SIDE");
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
