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
using zenith.Core.Helper;

namespace zenith.Core.Assimilation
{
    public class StatOutput
    {

        public StatType selectedStat = StatType.Strength; // TODO : Bind to GUI
        private readonly ZenithData zenithData;
        public Dictionary<StatType, float> OutputPercentages { get; private set; } = new()
        {
            {StatType.Strength, 100f },
            {StatType.Speed, 100f },
            {StatType.Jump, 100f },
            {StatType.Health, 100f },
            {StatType.AnimalLoot, 100f },
            {StatType.Harvesting, 100f },
            {StatType.Forage, 100f },
            {StatType.Stealth , 100f },
            {StatType.CropRate, 100f }
        };


        public enum StatType
        {
            Strength,
            Speed,
            Jump,
            Health,
            AnimalLoot,
            Harvesting,
            Forage,
            Stealth,
            CropRate
        }



        private readonly Entity entity;
         private EntityPlayer Player => entity as EntityPlayer;

        public StatOutput(Entity entity,ZenithData zenithData)
        {
            this.entity = entity;
            this.zenithData = zenithData;
            var entityPlayer = Player as EntityPlayer;


            LoadStatOutput();

            OnOutputChange += () =>
            {
                SaveStatOutput();
            };
        }

     

       


        public event Action OnOutputChange;
        public void StatSwitch(StatType statType) // Called with Client -> Server Packet
        {

            Logger.Log(Player,$"[FLOW] StatSwitch Called");

#pragma warning disable IDE0019
            var sapi = entity.World.Api as ICoreServerAPI;
            if (sapi == null) return;
#pragma warning restore IDE0019


            selectedStat = statType;

            Logger.Log(Player, $"[DATA] SSI : {selectedStat}");

            if (!Enum.IsDefined(typeof(StatType), selectedStat))
            {
                selectedStat = StatType.Strength;
            }

            OnOutputChange.Invoke();


            sapi.SendMessage(Player.Player, GlobalConstants.AllChatGroups, $"{selectedStat}", EnumChatType.Notification);

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

          

            float oldValue = OutputPercentages[selectedStat];

            float newValue = oldValue + amount;

            newValue = Math.Clamp(newValue, 0f, 100f);

            if (oldValue == newValue)
            {
                return;
            }

            OutputPercentages[selectedStat] = newValue;
            OnOutputChange?.Invoke();
            sapi.SendMessage(Player.Player, GlobalConstants.AllChatGroups, $"{selectedStat} Output: {OutputPercentages[selectedStat]}%", EnumChatType.Notification);

        }

        private void SaveStatOutput()
        {

            zenithData.Tree.SetInt("SelectedStat",(int)selectedStat);

            foreach (var Stat in OutputPercentages)
            {
                var key = Stat.Key;
                var value = Stat.Value;

                zenithData.Tree.SetFloat($"{key} Output", value);

        //       Log($"[SAVE]  {key} Output | Value : {value}");
            }

            

            entity.WatchedAttributes.MarkPathDirty("zenith");

        }

        private void LoadStatOutput()
        {
          selectedStat = (StatType)zenithData.Tree.GetInt("SelectedStat", (int)selectedStat);

            foreach (var Stat in OutputPercentages)
            {
                var key = Stat.Key;

               float value = zenithData.Tree.GetFloat($"{key} Output", 100f);

                OutputPercentages[key] = value;

           //     Log($"[Load] {key} Output : {value}");
            }


          //  Log($"Selected stat {selectedStat}");


            entity.WatchedAttributes.MarkPathDirty("zenith");

        }
        
      
    }
}
