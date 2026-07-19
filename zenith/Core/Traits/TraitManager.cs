using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core.Progression;

namespace zenith.Core.Traits
{
    public class TraitManager
    {


     public Traits Traits{ get; }
        private Assimilation.Assimilation Assimilation { get; }
        private readonly TreeAttribute watchedZenith;
        public static bool DebugMode => ZenithSettings.ZDebugMode;

        private EntityPlayer Player => entity as EntityPlayer;
        private readonly Entity entity;

        public TraitManager(Entity entity)
        {
            this.entity = entity;

            Traits = new Traits(entity);
          
        }

        public void TryApplyTraits( ) // Go more indepth later ie. Max Drifter
        {
        //    Log($"[FLOW] Can Use Traits Called!"); 

            if (Assimilation?.AssimStage < 1) // Might turn into IAssimilationProvider
            {
                Log($"[FLOW] AssimStage too low Returned!");
                return;
            }

            Traits.ApplyTraits();
         //   Log($"Player Name : {Player.Player.PlayerName}");

        }

        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }
    }
}
