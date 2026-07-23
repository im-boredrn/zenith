using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
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

        private StatType selectedStat = StatType.Strength;
        public float OutputPercent { get; set; }
        bool shiftHeld { get; set; }
        bool ctrlHeld { get; set; }


        static public bool DebugMode => ZenithSettings.ZDebugMode;

        private readonly Entity entity;
         private EntityPlayer Player => entity as EntityPlayer;
        private ICoreClientAPI capi;

        public StatOutput(Entity entity, ICoreClientAPI capi)
        {
            this.entity = entity;
            this.capi = capi;
        }

        string[] statNames =
        {
            "Strength",// 0
            "Speed", // 1
            "Jump",// 2

        };

        private enum StatType
        {
            Strength,
            Speed,
            Jump
        }


        public event Action OnOutputChange;
        public void StatSwitch() // Call with Keybind
        {

            Log($"[FLOW] StatSwitch Called");

#pragma warning disable IDE0019
            var sapi = entity.World.Api as ICoreServerAPI;
            if (sapi == null) return;
#pragma warning restore IDE0019


            selectedStat++;
           // Log($"[DATA] SSI : {SelectedStatIndex}");

            if ((int)selectedStat >= Enum.GetValues(typeof(StatType)).Length)
            {
                selectedStat = StatType.Strength;
            }


            sapi.SendMessage(Player.Player, GlobalConstants.AllChatGroups, $"{selectedStat}", EnumChatType.Notification);
           
           
            Log($"[DATA] SSI : {selectedStat} | Selected Stat : {selectedStat}");

        }

        public void OutputChange(bool shiftHeld, bool ctrlHeld, string intent)
        {
#pragma warning disable IDE0019
            var sapi = entity.World.Api as ICoreServerAPI;
            if (sapi == null) return;

#pragma warning restore IDE0019

            switch (intent)
            {
                case "Increase":
                    {

                     if (shiftHeld)
                     {
                            Log($"isShiftHeld? {shiftHeld}");
                            OutputPercent += 25f;
                      }
                        else  if (ctrlHeld)
                        {
                            OutputPercent += 1f;
                        }
                        else
                        {
                            OutputPercent += 10f;
                        }

                        if (OutputPercent > 100f)
                        {
                            OutputPercent = 100f;
                        }
                        OnOutputChange.Invoke();
                        break;
                    }

                case "Decrease":
                    {

                        if (shiftHeld)
                        {
                            OutputPercent -= 25f;
                        }

                        else if (ctrlHeld)
                        {
                            OutputPercent -= 1f;
                        }
                        else
                        {
                            OutputPercent -= 10f;
                        }

                        if (OutputPercent < 0f)
                        {
                            OutputPercent = 0f;
                        }
                        OnOutputChange.Invoke();
                        break;
                    }


            }

            sapi.SendMessage(Player.Player, GlobalConstants.AllChatGroups, $"Current Output: {OutputPercent}%", EnumChatType.Notification);

        }

       
        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }
    }
}
