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
using zenith.Config;
using zenith.Core.Adaptations;
using zenith.Core.AdaptationsCore.AdaptationsFactory;
using zenith.Core.Assimilation;
using zenith.Core.Helper;
using static zenith.Core.Adaptations.CreatureDefinition;
using CreatureType = zenith.Core.Adaptations.CreatureDefinition.CreatureType;
using AdaptationCategory = zenith.Core.AdaptationsCore.BlockDefinitions.BlockCategory;
namespace zenith.Core.AdaptationsCore
{
    public class CreatureAdaptations 
    {

      
        // Maybe make a list / dict of consumable blocks. I.e BlockDefinition.
        // Move Dict init somewhere it is a lot to scroll through.

        private Dictionary<Type, Func<Adaptation>> AdaptationProducer { get; } = [];
        public readonly List<Adaptation> ActiveAdaptations = [];
        public IReadOnlyDictionary<Type, Func<Adaptation>> AdaptationManager => AdaptationProducer;
        public BearSenses BearSenses { get; private set; }

        private EntityPlayer Player => entity as EntityPlayer;
        private readonly Entity entity;
        private readonly ZenithData zenithData;
        private TreeAttribute ZenithData => zenithData.Tree;
        // Always Check for Stale References -- I lost 1 day and 5 hours to this mistake.
        public event Action OnAdaptationChanged;

        public CreatureAdaptations(Entity entity, ZenithData data) 
        {
            this.entity = entity;
            this.zenithData = data;
            // this.core = coreAPI;
            if (Player == null)
            {
                throw new Exception("CreatureAdaptations attached to non-player entity");
            }
            AdaptationProducer = new Dictionary<Type, Func<Adaptation>>()
            {
                [typeof(WolfAdaptation)] = () => new WolfAdaptation(entity.World, entity, CreatureDefinition.CreatureLibrary),
                [typeof(BearSenses)] = () => new BearSenses(entity.World, entity as EntityPlayer, CreatureDefinition.CreatureLibrary),
                [typeof(ClayAdaptation)] = () => new ClayAdaptation(entity.World, entity, BlockDefinitions.BlockLibrary)

            };



          //  Log($"Tree Null? {ZenithData == null}");
            InitializeAdapt();

        }

        public Adaptation CreateCreatureAdaption(CreatureType creatureType )
        {
            var type = CreatureDefinition.CreatureLibrary[creatureType].AdaptationType;

            if (type != null && AdaptationProducer.TryGetValue(type, out var factory))
            {
                var adaptation = factory();

            //    Log($"Created {adaptation.GetType().Name} {adaptation.GetHashCode()}");

                return adaptation;
            }

            return null;
        }

        public Adaptation CreateBlockAdaptation(ItemStack stack)
        {
            var type = BlockDefinitions.BlockDefinition[stack.Collectible.Code].AdaptationType;

            if (type != null && AdaptationProducer.TryGetValue(type, out var factory))
            {
                var adaptation = factory();

                //    Log($"Created {adaptation.GetType().Name} {adaptation.GetHashCode()}");

                return adaptation;
            }

            return null;
        }

        public void CheckAdaptation(CreatureType creatureType)
        {

            var def = CreatureDefinition.CreatureLibrary[creatureType];
            var sapi = entity.World.Api as ICoreServerAPI;

            def.Counter += 1;
            if (def.Counter >= def.Threshold)
            {
                def.Counter = def.Threshold;
            }
            else
            {
                SaveCAdapt();
                return;

            }


            if (!def.IsLocked &&
                def.HasAdaptation )
            {

            //    Log("[LOAD] Creating adaptation");

                var adaptation = CreateCreatureAdaption(creatureType);

                string text = $"{creatureType} Adaptation Successfully Assimilated";

                if (!String.IsNullOrEmpty(text))
                {
                    text = $"{char.ToUpper(text[0])}{text[1..]}";
                }
               
                sapi.SendMessage(Player.Player, GlobalConstants.GeneralChatGroup,
                    text, EnumChatType.Notification);


                if (adaptation != null)
                {
                    RegisterAdaptation(adaptation);
                    def.IsLocked = true;
                }

            }

            SaveCAdapt();
           
        }

        public void EatItem(ItemStack stack)
        {
            var sapi = entity.World.Api as ICoreServerAPI;
            var code = stack.Collectible.Code;



            if (BlockDefinitions.BlockDefinition.TryGetValue(code, out var definitions))
            {
                definitions.BlockLVL += stack.StackSize;

                if (definitions.BlockLVL >= definitions.Threshold)
                {
                    definitions.BlockLVL = definitions.Threshold;
                }
                else
                {
                    SaveCAdapt();
                    return;

                }


                if (!definitions.IsLocked)
                {

                    var adaptation = CreateBlockAdaptation(stack);

                    string text = $"{definitions.AdaptationType.Name} Adaptation Successfully Assimilated";

                    if (!String.IsNullOrEmpty(text))
                    {
                        text = $"{char.ToUpper(text[0])}{text[1..]}";
                    }

                    sapi.SendMessage(Player.Player, GlobalConstants.GeneralChatGroup,
                        text, EnumChatType.Notification);


                    if (adaptation != null)
                    {
                        RegisterAdaptation(adaptation);
                        definitions.IsLocked = true;
                    }
                    SaveCAdapt();
                }
            }
            sapi.SendMessage(Player.Player, GlobalConstants.CurrentChatGroup, $"Assimilated {stack.GetName()}", EnumChatType.Notification);

            Logger.Log(Player, $"{stack.Collectible.Code}");

        }


        private void RegisterAdaptation(Adaptation adaptation)
        {

            if (ActiveAdaptations.Any(a => a.SourceCreature == adaptation.SourceCreature))
            {
             //   Log($"Duplicate prevented {adaptation.SourceCreature}");
                return;
            }


            if (adaptation is BearSenses bear)
            {
                if (BearSenses != null) return;
                BearSenses = bear;
            }
            
            ActiveAdaptations.Add(adaptation);
            ApplyAdaptations();
            OnAdaptationChanged?.Invoke();
            //Log($"SERVER COUNT AFTER ADD: {ActiveAdaptations.Count}");
        }

        public void AssimilateLink(CreatureType creatureType) 
        {
         //   Log("[CA-FLOW] AssimilateLink Called");
            var def = CreatureDefinition.CreatureLibrary[creatureType];
            foreach (var adaptation in ActiveAdaptations)
            {
                adaptation.OnAssimilate(entity, def, CreatureDefinition.CreatureLibrary);
            }
        }

      
        public void EvolveAdaptation(Adaptation adaptation)
        {
            if (!adaptation.ReadyToEvolve) return;

            adaptation.StageUp();
        }

        
       

        private void ApplyAdaptations()
        {
            foreach (var adaptation in ActiveAdaptations)
            {
                adaptation.Initialize();
            }
        }
        public void SaveCAdapt()
        {

            Logger.Log(Player,$"Saving {ActiveAdaptations.Count} adaptations");

            foreach (var adadpt in ActiveAdaptations)
            {
           //     Log($"SAVE {adadpt.SourceCreature} {adadpt.GetHashCode()}");

            }
            foreach (var creature in CreatureDefinition.CreatureDefinitions.Where(c => !c.Value.IsUnknown))
            {
                ZenithData.SetInt($"{creature.Key} CA-Counter", creature.Value.Counter) ;
            }

            foreach (var block in BlockDefinitions.BlockDefinition)
            {
                ZenithData.SetInt($"{block.Key} CA-BlockLVL", block.Value.BlockLVL);
            }

            var adaptationTree = new TreeAttribute();

            for (int i = 0; i < ActiveAdaptations.Count; i++)
            {
                adaptationTree.SetString(i.ToString(), ActiveAdaptations[i].SourceCreature.ToString());
                adaptationTree.SetString(i.ToString(), ActiveAdaptations[i].BlockCategory.ToString());
            }



            ZenithData["adaptations"] = adaptationTree;
            entity.WatchedAttributes.MarkPathDirty("zenith");
            entity.WatchedAttributes.MarkPathDirty("adaptations");


        }

       

        public void InitializeAdapt()
        {

            ReloadAdapt();
        }

        public void ReloadAdapt()
        {
            ActiveAdaptations.Clear();
            BearSenses = null;


            var currentZenith = ZenithData;

            var adaptationTree = currentZenith?.GetTreeAttribute("adaptations");

            if (adaptationTree != null)
            {
                foreach (var key in adaptationTree)
                {
                    var creatureType = Enum.Parse<CreatureType>(adaptationTree.GetString(key.Key));

                    var def = CreatureDefinition.CreatureDefinitions[creatureType];
                    def.IsLocked = true;

                    //       Log("[LOAD] Creating adaptation");
                    var adaptation = CreateCreatureAdaption(creatureType);
                    if (adaptation is BearSenses bear)
                    {
                        BearSenses = bear;
                    }

                    if (adaptation != null)
                    {
                        ActiveAdaptations.Add(adaptation);
                    }
                }
            }
            foreach (var creature in CreatureDefinition.CreatureDefinitions.Where(c => !c.Value.IsUnknown))
            {
                creature.Value.Counter = ZenithData.GetInt($"{creature.Key} CA-Counter", 0);
            }

            foreach (var block in BlockDefinitions.BlockDefinition)
            {
                block.Value.BlockLVL = ZenithData.GetInt($"{block.Key} CA-BlockLVL", 0);
            }
        }
    }
}