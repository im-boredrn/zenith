using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using zenith.Config;
using zenith.Core.Assimilation;

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

            Log($"[FLOW] StatSwitch Called");

#pragma warning disable IDE0019
            var sapi = entity.World.Api as ICoreServerAPI;
            if (sapi == null) return;
#pragma warning restore IDE0019


            SelectedStatIndex++;
            Log($"[DATA] SSI : {SelectedStatIndex}");

            if (SelectedStatIndex >= statNames.Length)
            {
                SelectedStatIndex = 0;
            }

            var selectedStat = MapArray(SelectedStatIndex);

            sapi.SendMessage(Player.Player, GlobalConstants.AllChatGroups, $"{selectedStat}", EnumChatType.Notification);
           
           
            Log($"[DATA] SSI : {SelectedStatIndex} | Selected Stat : {selectedStat}");

        }

        public void OutputChange(string intent)
        {
#pragma warning disable IDE0019
            var sapi = entity.World.Api as ICoreServerAPI;
            if (sapi == null) return;
#pragma warning restore IDE0019

            switch (intent)
            {
                case "Increase":
                    {
                        OutputPercent += 10;
                        break;
                    }

                case "Decrease":
                    {
                        OutputPercent -= 10; break;
                    }
            }

            sapi.SendMessage(Player.Player, GlobalConstants.AllChatGroups, $"Current Output: {OutputPercent}%", EnumChatType.Notification);

        }

        private string MapArray(int SSI)
        {

            if (SSI < 0 || SSI >= statNames.Length) return "out of bounds";

            return statNames[SSI];

        }
        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }
    }
}
