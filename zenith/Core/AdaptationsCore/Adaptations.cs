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

            FullAdaptations = [];

            InitializeActiveAdaptations();


            if (entity.World.Side == EnumAppSide.Server)
            {
                var sapi = entity?.World?.Api as ICoreServerAPI;

                sapi?.Event?.HandInteract += Event_HandInteract;

            }
            //  Log($"Tree Null? {ZenithData == null}");
            InitializeAdapt();

        }

        private void Event_HandInteract(IServerPlayer player, EnumHandInteractNw enumHandInteract, float secondsPassed, ref EnumHandling handling)
        {
            var itemSlot = player.Entity.RightHandItemSlot;

            if (itemSlot.Empty)
                return;

            if (!itemSlot.Itemstack.GetName().Contains("clay"))
                return;

            if (!FullAdaptations.TryGetValue(typeof(ClayDefinition), out var adaptation))
                return;

            if (!adaptation.IsUnlocked)
                return;

            if (adaptation.Behavior is not ClayBehavior clayBehavior)
                return;

            if (clayBehavior.HealWithClay(Player,itemSlot))
            {
                handling = EnumHandling.PreventDefault;
            }
        }

        public ActiveAdaptation Get<T>() where T : AdaptationDefinitions
        {
            return FullAdaptations[typeof(T)];
        }
        public bool TryGet(Type type,out ActiveAdaptation adaptation)
        {
            return FullAdaptations.TryGetValue(type, out adaptation);
        }


        private void Register<TDefinition,TState>(TState state, TDefinition definitions,AdaptationBehavior behavior)
            where TDefinition : AdaptationDefinitions
            where TState : AdaptationState
        {
            PlayerStates[typeof(TDefinition)] = state;

            FullAdaptations[typeof(TDefinition)] = new ActiveAdaptation(state, definitions, behavior);
        }

        private void InitializeActiveAdaptations()
        {
            var entityPlayer = entity as EntityPlayer;
            var saves = PlayerStates;

            WolfState wolfState = new ();
            BearState bearState = new (); // Parallel Vars do a dictionary
            ClayState clayState = new ();

            Register(wolfState , new
                WolfDefinition(wolfState), new WolfBehavior(entityPlayer));

            Register(bearState , new
                BearSensesDefinition(bearState),
                new BearBehavior( entityPlayer, bearState));

            Register(clayState, new ClayDefinition(clayState),
                new ClayBehavior(clayState));
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




            
            foreach (var (entry, state) in PlayerStates)
            {
               state.BlockLVL = ZenithData.GetInt($"{entry} CA-BlockLVL", 0);
                state.Counter = ZenithData.GetInt($"{entry} CA-Counter", 0);
                state.IsUnlocked = ZenithData.GetBool($"{entry}", false);

            }
        }
    }
}