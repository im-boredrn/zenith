using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using zenith.Config;
using static zenith.Core.Assimilation.AssimilationCore;

namespace zenith.Core.Adaptations
{
    public class BearSenses  : Adaptation, ITickable
    {

       
        private readonly IWorldAccessor world;
        private readonly EntityPlayer Player;
        static public bool DebugMode => ZenithSettings.ZDebugMode;


        public BearSenses(IWorldAccessor world, EntityPlayer entity) : base(world, entity) // Bear Sense, Pack Mule
        {
            this.Player = entity;
            this.world = world;
            //Log($"[BEAR] Created {this.GetHashCode()}");
            Log($"Side: {entity.World.Side}");


        }

        private float SenseTimer;
        private float DebugTimer;
        private readonly List<SensedEntity> SensedEntities = new();

        public IReadOnlyList<SensedEntity> sensedEntities => SensedEntities;

      


        public  void OnTick(float dt)
        {
            if (entity.World.Side != EnumAppSide.Client)
                return;


            foreach (var sensed in SensedEntities)
            {
                sensed.Timer -= dt;
            }

            SensedEntities.RemoveAll(s => s.Timer <= 0);
            SenseTimer += dt;
            DebugTimer += dt;
            if (SenseTimer > 0.1f)
            {
                UpdateSense();

              //  Log($"[BEAR] Sensed count after update: {sensedEntities.Count}");
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

            var capi = entity.World.Api as ICoreClientAPI;


           
            //SensedEntities.Clear();
            foreach (var entity in entities)
            {

                if ($"{entity.Code}".Contains("player") || entity.Code.ToString().Contains("item")) continue;
                if (!entity.Alive) continue;

                SensedEntity existing = null;

                foreach (var sensed in SensedEntities)
                {
                    if (sensed.EntityID == entity.EntityId)
                    {
                        existing = sensed;
                        break;

                    }
                }

                double distance = entity.Pos.DistanceTo(Player.Pos.XYZ);
                Vec3d relative = entity.Pos.XYZ - Player.Pos.XYZ;
                Vec3d direction = relative.Clone().Normalize();
                Vec3d worldPos = entity.Pos.XYZ.AddCopy(0, entity.SelectionBox.Y2, 0);
                double side = lookDir.Cross(direction).Y;
                double dot = direction.Dot(lookDir);
                bool hostile = IsPredator(entity);
                if (existing != null)
                {
                    existing.Timer = 5f;
                    existing.WorldPosition = worldPos;
                    existing.Direction = direction;
                    existing.Dot = dot;
                    existing.Distance = distance;
                    existing.Side = side;
                    existing.Hostile = hostile;
                }
                else
                {
                    SensedEntities.Add(new SensedEntity
                    {

                        EntityID = entity.EntityId,
                        Code = entity.Code.ToString(),
                        Direction = direction,
                        WorldPosition = worldPos,
                        Distance = distance,
                        Dot = dot,
                        Timer = 5f,
                        Side = side,
                        Hostile = hostile
                        

                    });
                }

}
        }

        private void DebugSense()
        {


         //   if (Player == null) return;
         //   var sapi = entity.World.Api as ICoreServerAPI;
            StringBuilder senseOutput = new StringBuilder();

            foreach (var sensed in SensedEntities)
            {
            //    if (!sensed.Entity.Alive) continue;


                double angle = Math.Acos(sensed.Dot) * GameMath.RAD2DEG;
            //    senseOutput.AppendLine($"{sensed.Code} {sensed.Direction} Dot : {sensed.Dot:F2} - {angle:F0} Degrees | {GetDirection(sensed.Dot)}");

            }
            //Log(senseOutput.ToString());
            //sapi.SendMessage(Player.Player, GlobalConstants.AllChatGroups,
            //     senseOutput.ToString(), EnumChatType.Notification);
        }

        public class SensedEntity
        {

            //      public Entity Entity;
            public long EntityID;
            public string Code;
            public Vec3d Direction;
            public Vec3d WorldPosition;

            public double Distance;
            public double Dot;
            public double Side;

            public float Timer = 5f;
            public bool Hostile;
        }
  string GetDirection(double dot)
        {
            if (dot > 0.5f)
            {
                return ("FRONT");
            }
            else if (dot < -0.5f)
            {
                return ("BEHIND");
            }
            else
            {
                return ("SIDE");
            }
        }

        bool IsPredator(Entity entity)
        {

            if (entity is EntityAgent)
            {
                if (entity.Code.ToString().Contains("bear") || entity.Code.ToString().Contains("wolf") ||
                    entity.Code.ToString().Contains("hyena") ||
                    entity.Code.ToString().Contains("drifter") ||
                    entity.Code.ToString().Contains("bowtorn") ||
                    entity.Code.ToString().Contains("shiver"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
        public override CreatureType SourceCreature => CreatureType.bear;


        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }
    }



}
