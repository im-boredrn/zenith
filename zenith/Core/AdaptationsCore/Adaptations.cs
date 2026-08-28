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
using zenith.Core.AdaptationsCore.Adaptation_Definitions;
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

      
        private readonly Dictionary<Type, AdaptationState> PlayerStates = [];


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

            InitializeAdapt();

            //if (entity.World.Side == EnumAppSide.Server)
            //{
            //    var sapi = entity?.World?.Api as ICoreServerAPI;

            //    sapi?.Event?.HandInteract += Event_HandInteract;

            //}

        }

        //private void Event_HandInteract(IServerPlayer player, EnumHandInteractNw enumHandInteract, float secondsPassed, ref EnumHandling handling)//  multi-call
        //{
        //    var itemSlot = player.Entity.RightHandItemSlot;

        //    if (itemSlot.Empty)
        //        return;

        //}

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

        private void InitializeActiveAdaptations() // Should Probably add an unlearn behavior in GUI. On Clicked send packet --> clicked behavior.IsUnlocked = false;
        {
            var entityPlayer = entity as EntityPlayer;

            WolfState wolfState = new ();
            BearState bearState = new (); // Parallel Vars do a dictionary -- maybe add them to a list and iterate through it , returning state.
            ClayState clayState = new ();
            PoisonState poisonState = new();

            Register(wolfState , new
                WolfDefinition(wolfState), new WolfBehavior(entityPlayer));

            Register(bearState , new
                BearSensesDefinition(bearState),
                new BearBehavior( entityPlayer, bearState));

            Register(clayState, new ClayDefinition(clayState),
                new ClayBehavior(clayState));

            Register(poisonState, new PoisonDefinition(poisonState)
                , new PoisonBehavior(poisonState));
        }
        
       

        public void CheckAdaptation(CreatureType creatureType)
        {
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

                if (entity.World.Api is ICoreClientAPI capi)
                {
                    ZenithCore.PlayPlayerSound(capi, "sounds/assimilation/adaptationgained", Player, 0.8f, 1f);
                }
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
                // #TODO Play Sound

                var definitions = match.Value;

                if (definitions.AdaptationType == null)
                    return;

                var state = PlayerStates[definitions.AdaptationType];

                state.BlockLVL += stack.StackSize;


                var clay = Get<ClayDefinition>();

                if (clay.Behavior is ClayBehavior behavior)
                {
                    behavior.CanAbsorb(Player, stack);
                }

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
            var clay = Get<ClayDefinition>();
            
            if (wolf.Behavior is WolfBehavior behavior && wolf.IsUnlocked && !clay.IsUnlocked)
            {
                behavior.OnAssimilate(creatureType);
            }
           
        }

       
        private void SavePlayerProgress()
        {

            Logger.Log(Player,$"Saving adaptations");

           foreach (var adaptation in FullAdaptations.Values)
            {
                adaptation.State.Save(ZenithData);
            }
          
           
          

            entity.WatchedAttributes.MarkPathDirty("zenith");
         
        }

        private void InitializeAdapt()
        {

            ReloadAdapt();
        }

        public void ReloadAdapt()
        {


            foreach (var adaptation in FullAdaptations.Values)
            {
                adaptation.State.Load(ZenithData);
            }
        
        }
    }
}