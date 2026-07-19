using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core.Assimilation;
using zenith.Core.Progression;
using static zenith.Core.Assimilation.Assimilation;
using CreatureType = zenith.Core.Assimilation.Assimilation.CreatureType;

namespace zenith.Core.Traits
{
    public class TraitManager
    {


     public Traits Traits{ get; }
        private readonly TreeAttribute watchedZenith;
        public static bool DebugMode => ZenithSettings.ZDebugMode;

        private EntityPlayer Player => entity as EntityPlayer;
        private readonly Entity entity;
        private readonly IAssimilationProvider assimilationProvider;

        public TraitManager(Entity entity, IAssimilationProvider assimilationProvider)
        {
            this.assimilationProvider = assimilationProvider;
            this.entity = entity;

            Traits = new Traits(entity, assimilationProvider);
          
        }

        public void CalcTraits()
        {
            assimilationProvider.GetCreatureLevel(CreatureType.drifter) 
       
        }

        public void TryApplyTraits( ) // Go more indepth later ie. Drifter LVL, Wolf LVL
        {
        //    Log($"[FLOW] Can Use Traits Called!"); 

           // if (eventually get creature level here () < 1) // Might turn into IAssimilationProvider
            {
                Log($"[FLOW] AssimStage too low Returned!");
                return;
            }

         //   Traits.ApplyTraits();
         //   Log($"Player Name : {Player.Player.PlayerName}");

        }

        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }
    }
}
