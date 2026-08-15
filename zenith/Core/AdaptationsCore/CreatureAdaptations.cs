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
    public class CreatureAdaptations 
    {

      
        // Maybe make a list / dict of consumable blocks. I.e BlockDefinition.
        // Move Dict init somewhere it is a lot to scroll through.

        private Dictionary<Type, Func<AdaptationDefinitions>> AdaptationProducer { get; } = [];
        public readonly List<AdaptationDefinitions> ActiveAdaptations = [];
        public readonly List<string> BlockAdaptations = [];
        private readonly Dictionary<string, AdaptationProgress> PlayerAdaptationProgress = [];
        public IReadOnlyDictionary<Type, Func<AdaptationDefinitions>> AdaptationManager => AdaptationProducer;
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
            PlayerAdaptationProgress = AdaptationProgress.CreateProgress();

            AdaptationProducer = new Dictionary<Type, Func<AdaptationDefinitions>>()
            {
                [typeof(WolfAdaptation)] = () => new WolfAdaptation(entity.World, entity, PlayerAdaptationProgress),
                [typeof(BearSenses)] = () => new BearSenses(entity.World, entity as EntityPlayer, PlayerAdaptationProgress),
                [typeof(ClayAdaptation)] = () => new ClayAdaptation(entity.World, entity, PlayerAdaptationProgress)

            };


          //  Log($"Tree Null? {ZenithData == null}");
            InitializeAdapt();

        }

     
        public void CheckAdaptation(CreatureType creatureType)
        {
            string key = creatureType.ToString();
            var def = PlayerAdaptationProgress[key];

            var creaturedef = CreatureDefinitions[creatureType];

            var sapi = entity.World.Api as ICoreServerAPI;

            def.Counter += 1;
            if (def.Counter >= creaturedef.Threshold)
            {
                def.Counter = creaturedef.Threshold;
            }
            else
            {
                SavePlayerProgress();
                OnAdaptationChanged?.Invoke();
                return;

            }


            if (!def.IsUnlocked &&
                creaturedef.HasAdaptation ) // Unlock 
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
                    def.IsUnlocked = true;
                }

            }

            SavePlayerProgress();
           
        }

        public void EatItem(ItemStack stack)
        {
            var sapi = entity.World.Api as ICoreServerAPI;
            var code = stack.Collectible.Code.ToString();

            var match = BlockDefinitions.BlockLibrary.FirstOrDefault(x => code.Contains(x.Key));

            if (match.Key != null)
            {
                var definitions = match.Value;
                var playerProgress = PlayerAdaptationProgress[match.Key];
                playerProgress.BlockLVL += stack.StackSize;
                Logger.Log(Player, $"{playerProgress.BlockLVL}");
                if (playerProgress.BlockLVL >= definitions.Threshold)
                {
                    playerProgress.BlockLVL = definitions.Threshold;
                }
                else
                {
                    SavePlayerProgress();
                    OnAdaptationChanged?.Invoke();
                    return;

                }

                var adaptation = CreateBlockAdaptation(match.Key);

                if (!adaptation.IsUnlocked)
                {


                    string text = $"{adaptation.AdaptationName}  Successfully Assimilated";

                    if (!String.IsNullOrEmpty(text))
                    {
                        text = $"{char.ToUpper(text[0])}{text[1..]}";
                    }

                    sapi.SendMessage(Player.Player, GlobalConstants.GeneralChatGroup,
                        text, EnumChatType.Notification);


                    if (adaptation != null)
                    {
                        RegisterAdaptation(adaptation);
                        adaptation.IsUnlocked = true;
                    }
                    SavePlayerProgress();
                    OnAdaptationChanged?.Invoke();
                }
            }
            sapi.SendMessage(Player.Player, GlobalConstants.CurrentChatGroup, $"Assimilated {stack.GetName()}", EnumChatType.Notification);

            Logger.Log(Player, $"{stack.Collectible.Code}");

        }
   public AdaptationDefinitions CreateCreatureAdaption(CreatureType creatureType )
        {
            var type = CreatureDefinition.CreatureLibrary[creatureType].AdaptationType;

            if (type != null && AdaptationProducer.TryGetValue(type, out var factory))
            {
                var adaptation = factory();


                return adaptation;
            }

            return null;
        }

        public AdaptationDefinitions CreateBlockAdaptation(string block)
        {

            var match = BlockDefinitions.BlockLibrary[block];
            if (match.AdaptationType != null && AdaptationProducer.TryGetValue(match.AdaptationType, out var factory))
            {
                var adaptation = factory();

                //    Log($"Created {adaptation.GetType().Name} {adaptation.GetHashCode()}");

                return adaptation;
            }

            return null;
        }


        private void RegisterAdaptation(AdaptationDefinitions adaptation)
        {
            if (adaptation.AdaptationCategory == AdaptCategory.AdaptationCategory.Creature)
            {
                if (ActiveAdaptations.Any(a => a.SourceCreature == adaptation.SourceCreature))
                    return;

                ActiveAdaptations.Add(adaptation);

            }


            if (adaptation.AdaptationCategory == AdaptCategory.AdaptationCategory.Block)
            {
                if (BlockAdaptations.Contains(adaptation.BlockCode))
                    return;

                BlockAdaptations.Add(adaptation.BlockCode);
            }

            if (adaptation is BearSenses bear)
            {
                if (BearSenses != null) return;
                BearSenses = bear;
            }

            
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
            } // Remove Dependency and route into primary constructor
        }

      
        public void EvolveAdaptation(AdaptationDefinitions adaptation)
        {
           

            adaptation.StageUp();
        }

        
       

        private void ApplyAdaptations()
        {
            foreach (var adaptation in ActiveAdaptations)
            {
                adaptation.Initialize();
            }
        }
        private void SavePlayerProgress()
        {

            Logger.Log(Player,$"Saving {ActiveAdaptations.Count} adaptations");

            foreach (var entry in PlayerAdaptationProgress)
            {
                ZenithData.SetInt($"{entry.Key} CA-BlockLVL", entry.Value.BlockLVL);
                ZenithData.SetInt($"{entry.Key} CA-Counter", entry.Value.Counter);
                ZenithData.SetBool($"{entry.Key}", entry.Value.IsUnlocked);

            }

            var creatureAdaptationTree = new TreeAttribute();

            int creatureIndex = 0;

            foreach (var adaptation in ActiveAdaptations)
            {
                if (adaptation.AdaptationCategory == AdaptCategory.AdaptationCategory.Creature)
                {
                    creatureAdaptationTree.SetString(
                        creatureIndex.ToString(),
                        adaptation.SourceCreature.ToString());

                    creatureIndex++;
                }
            }

            var blockAdaptationTree = new TreeAttribute();

            for (int i = 0; i < BlockAdaptations.Count; i++)
            {
                blockAdaptationTree.SetString(
                    i.ToString(),
                    BlockAdaptations[i]);
            }


            ZenithData["creatureAdaptations"] = creatureAdaptationTree;

            ZenithData["blockAdaptations"] = blockAdaptationTree;

            entity.WatchedAttributes.MarkPathDirty("zenith");
            entity.WatchedAttributes.MarkPathDirty("creatureAdaptations");
            entity.WatchedAttributes.MarkPathDirty("blockAdaptations");

        }

        private void InitializeAdapt()
        {

            ReloadAdapt();
        }

        public void ReloadAdapt()
        {
            ActiveAdaptations.Clear();
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

                    var adaptation = CreateCreatureAdaption(creatureType);

                    if (adaptation is BearSenses bear)
                        BearSenses = bear;

                    if (adaptation != null)
                        ActiveAdaptations.Add(adaptation);
                }
            }

            var blockTree = currentZenith?.GetTreeAttribute("blockAdaptations");

            if (blockTree != null)
            {
                foreach (var entry in blockTree) // get stored entries
                {
                    string code = entry.Value.ToString();
                    Logger.Log(Player, $"{code} Adaptation");

                    if (!BlockDefinitions.BlockDefinition.TryGetValue(code, out var definition)) // Retrieve value from entry
                        continue;


                    var adaptation = CreateBlockAdaptation(code); // recreate adaptation

                    if (adaptation != null)
                        ActiveAdaptations.Add(adaptation);

                    BlockAdaptations.Add(code);
                }
            }

            foreach (var entry in PlayerAdaptationProgress)
            {
               entry.Value.BlockLVL = ZenithData.GetInt($"{entry.Key} CA-BlockLVL", 0);
               entry.Value.Counter = ZenithData.GetInt($"{entry.Key} CA-Counter", 0);
                entry.Value.IsUnlocked = ZenithData.GetBool($"{entry.Key}", false);

            }
        }
    }
}