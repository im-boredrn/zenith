using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using zenith.Config;

namespace zenith.Core.NetWork
{
    public class StatOutput
    {

        public int SelectedStatIndex { get; set; }
        public int OutputPercent { get; set; }



        static public bool DebugMode => ZenithSettings.ZDebugMode;

        private readonly Entity entity;
         private EntityPlayer Player => entity as EntityPlayer;


        public StatOutput(Entity entity)
        {
            this.entity = entity;
        }

        string[] statNames =
        {
            "Strength",// 0
            "Speed", // 1
            "Jump",// 2

        };



        public void StatSwitch() // Call with Keybind
        {

            if (SelectedStatIndex >= statNames.Length)
            {
                SelectedStatIndex = 0;
            }

            var serverPlayer = Player.Api as IServerPlayer;

            SelectedStatIndex++;

            
            var selectedStat = MapArray(SelectedStatIndex);

            serverPlayer.SendLocalisedMessage(2, $"{selectedStat}");
            Log($"[DATA] SSI : {SelectedStatIndex}");
           
           
            Log($"[DATA] SSI : {SelectedStatIndex} | Selected Stat : {statNames}");

           


        }

        private string MapArray(int SSI)
        {
            return statNames[SSI];

        }
        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }
    }
}
