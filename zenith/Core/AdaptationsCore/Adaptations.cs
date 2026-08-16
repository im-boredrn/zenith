using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using Vintagestory.ServerMods;
using zenith.Config;
using zenith.Core.AdaptationsCore.AdaptationBehaviors;
using zenith.Core.AdaptationsCore.AdaptationData;
using zenith.Core.AdaptationsCore.AdaptationsFactory;
using zenith.Core.Assimilation;
using zenith.Core.Definitions;
using zenith.Core.Helper;
using static zenith.Core.Definitions.CreatureDefinition;
using AdaptationCategory = zenith.Core.Definitions.BlockDefinitions.BlockCategory;
using CreatureType = zenith.Core.Definitions.CreatureDefinition.CreatureType;
namespace zenith.Core.AdaptationsCore
{
    public class Adaptations 
    {

      
        public readonly List<string> BlockAdaptations = [];
        private readonly Dictionary<Type, AdaptationState> PlayerStates = [];
        public BearSensesDefinition BearSenses { get; private set; }  // Maybe Switchto Behavior


        private EntityPlayer Player => entity as EntityPlayer;
        private readonly Entity entity;
        private readonly ZenithData zenithData;
        private TreeAttribute ZenithData => zenithData.Tree;
        // Always Check for Stale References -- I lost 1 day and 5 hours to this mistake.
        public event Action OnAdaptationChanged;
        public readonly Dictionary<Type, ActiveAdaptation> FullAdaptations;

        public Adaptations(Entity entity, ZenithData data) 
        {
            this.entity = entity;
            this.zenithData = data;
            if (Player == null)
            {
                throw new Exception("CreatureAdaptations attached to non-player entity");
            }
            PlayerStates = AdaptationState.CreateProgress();

            FullAdaptations = [];

            InitializeActiveAdaptations();
         


          //  Log($"Tree Null? {ZenithData == null}");
            InitializeAdapt();

        }

        public ActiveAdaptation Get<T>() where T : AdaptationDefinitions
        {
            return FullAdaptations[typeof(T)];
        }
        public bool TryGet(Type type,out ActiveAdaptation adaptation)
        {
            return FullAdaptations.TryGetValue(type, out adaptation);
        }


        private void Register<T>(AdaptationState state, T definitions,AdaptationBehavior behavior) where T : AdaptationDefinitions
        {
            FullAdaptations[typeof(T)] = new ActiveAdaptation(state, definitions, behavior);
        }

        private void InitializeActiveAdaptations()
        {
            var world = entity.World;
            var entityPlayer = entity as EntityPlayer;
            var saves = PlayerStates;
            Register(saves[typeof(WolfDefinition)], new
                WolfDefinition( saves[typeof(WolfDefinition)]), new WolfBehavior(entityPlayer));

            Register(saves[typeof(BearSensesDefinition)], new
                BearSensesDefinition( saves[typeof(BearSensesDefinition)]),
                new BearBehavior( entityPlayer, saves[typeof(BearSensesDefinition)]));

           

            Register(saves[typeof(ClayDefinition)], new
                ClayDefinition( saves[typeof(ClayDefinition)]), new ClayBehavior());
        }

       

        public void CheckAdaptation(CreatureType creatureType)
        {
            string matc = creatureType.ToString();
            var creatureDef = CreatureDefinitions[creatureType];
            if (creatureDef.AdaptationType == null)
                return;

            var state = PlayerStates[creatureDef.AdaptationType];
          
            var sapi = entity.World.Api as ICoreServerAPI;

            state.Counter += 1;
            if (state.Counter >= creatureDef.Threshold)
            {
                state.Counter = creatureDef.Threshold;
            }
            else
            {
                SavePlayerProgress();
                OnAdaptationChanged?.Invoke();
                return;

            }

            if (!state.IsUnlocked &&
                creatureDef.HasAdaptation ) // Unlock 
            {
                string text = $"{creatureType} Adaptation Successfully Assimilated";

                if (!String.IsNullOrEmpty(text))
                {
                    text = $"{char.ToUpper(text[0])}{text[1..]}";
                }

                sapi.SendMessage(Player.Player, GlobalConstants.GeneralChatGroup,
                    text, EnumChatType.Notification);
                    state.IsUnlocked = true;
            }

            SavePlayerProgress();
            OnAdaptationChanged?.Invoke(); // if null do these even send?
        }

        public void EatItem(ItemStack stack)
        {
            var sapi = entity.World.Api as ICoreServerAPI;
            var code = stack.Collectible.Code.ToString();

            var match = BlockDefinitions.BlockLibrary.FirstOrDefault(x => code.Contains(x.Key));

            if (match.Key != null)
            {
                var definitions = match.Value;

                if (definitions.AdaptationType == null)
                    return;
                var state = PlayerStates[definitions.AdaptationType];
                state.BlockLVL += stack.StackSize;
                Logger.Log(Player, $"{state.BlockLVL}");
                if (state.BlockLVL >= definitions.Threshold)
                {
                    state.BlockLVL = definitions.Threshold;
                }
                else
                {
                    SavePlayerProgress();
                    OnAdaptationChanged?.Invoke();
                    return;

                }


                if (!state.IsUnlocked)
                {


                    TryGet(definitions.AdaptationType, out var adaptation);

                    string text = $"{adaptation.Definitions.AdaptationName}  Successfully Assimilated";

                    if (!String.IsNullOrEmpty(text))
                    {
                        text = $"{char.ToUpper(text[0])}{text[1..]}";
                    }
                    
                    sapi.SendMessage(Player.Player, GlobalConstants.GeneralChatGroup,
                        text, EnumChatType.Notification);

                        state.IsUnlocked = true;
                    
                    SavePlayerProgress();
                    OnAdaptationChanged?.Invoke();
                }
            }
            sapi.SendMessage(Player.Player, GlobalConstants.CurrentChatGroup, $"Assimilated {stack.GetName()}", EnumChatType.Notification);

            Logger.Log(Player, $"{stack.Collectible.Code}");

        }
  

  
        public void AssimilateLink(CreatureType creatureType) 
        {
            var wolf = Get<WolfDefinition>();

            if (wolf.Behavior is WolfBehavior behavior && wolf.IsUnlocked)
            {
                behavior.OnAssimilate(creatureType);
            }
           
        }

       
        private void SavePlayerProgress()
        {

            Logger.Log(Player,$"Saving adaptations");

            foreach (var entry in PlayerStates)
            {
                ZenithData.SetInt($"{entry.Key} CA-BlockLVL", entry.Value.BlockLVL);
                ZenithData.SetInt($"{entry.Key} CA-Counter", entry.Value.Counter);
                ZenithData.SetBool($"{entry.Key}", entry.Value.IsUnlocked);

            }

     
            entity.WatchedAttributes.MarkPathDirty("zenith");
         
        }

        private void InitializeAdapt()
        {

            ReloadAdapt();
        }

        public void ReloadAdapt()
        {
            
            BlockAdaptations.Clear();
            BearSenses = null;


            var currentZenith = ZenithData;
            var creatureTree = currentZenith?.GetTreeAttribute("creatureAdaptations");

            if (creatureTree != null)
            {
                foreach (var entry in creatureTree)
                {
                    var creatureType =
                        Enum.Parse<CreatureType>(entry.Value.ToString());

                }
            }

            
            foreach (var entry in PlayerStates)
            {
               entry.Value.BlockLVL = ZenithData.GetInt($"{entry.Key} CA-BlockLVL", 0);
               entry.Value.Counter = ZenithData.GetInt($"{entry.Key} CA-Counter", 0);
                entry.Value.IsUnlocked = ZenithData.GetBool($"{entry.Key}", false);

            }
        }
    }
}