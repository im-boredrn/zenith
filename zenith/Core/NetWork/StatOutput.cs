using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core.Assimilation;

namespace zenith.Core.NetWork
{
    public class StatOutput
    {

        private StatType selectedStat = StatType.Strength;
        private readonly TreeAttribute watchedZenith;

        public Dictionary<StatType, float> OutputPercentages { get; private set; } = new()
        {
            {StatType.Strength, 100f },
            {StatType.Speed, 100f },
            {StatType.Jump, 100f }
        };
    

        static public bool DebugMode => ZenithSettings.ZDebugMode;


        private readonly Entity entity;
         private EntityPlayer Player => entity as EntityPlayer;
        private ICoreClientAPI capi;

        public StatOutput(Entity entity, ICoreClientAPI capi)
        {
            this.entity = entity;
            this.capi = capi;
            var entityPlayer = Player as EntityPlayer;

            watchedZenith = (TreeAttribute)(entityPlayer.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entityPlayer.WatchedAttributes["zenith"] = watchedZenith;

            LoadStatOutput();

            OnOutputChange += () =>
            {
                SaveStatOutput();
            };
        }

     

        public enum StatType
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

            if (!Enum.IsDefined(typeof(StatType), selectedStat))
            {
                selectedStat = StatType.Strength;
            }

            OnOutputChange.Invoke();

            sapi.SendMessage(Player.Player, GlobalConstants.AllChatGroups, $"{selectedStat}", EnumChatType.Notification);
           
           
            //Log($"[DATA] SSI : {selectedStat} | Selected Stat : {selectedStat}");

        }

        public void OutputChange(bool shiftHeld, bool altHeld, string intent)
        {
#pragma warning disable IDE0019
            var sapi = entity.World.Api as ICoreServerAPI;
            if (sapi == null) return;

#pragma warning restore IDE0019


            float amount = 10f;

            if (shiftHeld)
            {
                amount = 25f;
            }

            else if (altHeld)
            {
                amount = 1f;
            }

            if (intent == "Decrease")
            {
                amount *= -1;
            }

            OutputPercentages[selectedStat] += amount;

            OutputPercentages[selectedStat] = Math.Clamp(OutputPercentages[selectedStat], 0f, 100f);
            OnOutputChange.Invoke();
            sapi.SendMessage(Player.Player, GlobalConstants.AllChatGroups, $"Current Output: {OutputPercentages[selectedStat]}%", EnumChatType.Notification);

        }

        private void SaveStatOutput()
        {

            watchedZenith.SetInt("SelectedStat",(int)selectedStat);

            foreach (var Stat in OutputPercentages)
            {
                var key = Stat.Key;
                var value = Stat.Value;

                watchedZenith.SetFloat($"{key} Output", value);

               Log($"[SAVE]  {key} | Value : {value}");
            }
            entity.WatchedAttributes.MarkPathDirty("zenith");

        }

        private void LoadStatOutput()
        {
          selectedStat = (StatType)watchedZenith.GetInt("SelectedStat", (int)selectedStat);

            foreach (var Stat in OutputPercentages)
            {
                var key = Stat.Key;

               float value = watchedZenith.GetFloat($"{key} Output", 100f);

                OutputPercentages[key] = value;

                Log($"[Load] {key} Output : {value}");
            }


            Log($"Selected stat {selectedStat}");
            entity.WatchedAttributes.MarkPathDirty("zenith");

        }

        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }
    }
}
