using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using zenith.Config;
using zenith.Core.Adaptations;
using zenith.Core.AdaptationsCore.AdaptationsFactory;
using zenith.Core.Helper;
using CreatureType = zenith.Core.Assimilation.AssimilationCore.CreatureType;

namespace zenith.Core.AdaptationsCore
{
    public class CreatureAdaptations 
    {

        public Dictionary<CreatureType, CreatureDefinition> CreatureDefinitions { get; } = new Dictionary<CreatureType, CreatureDefinition>()
        {
            [CreatureType.drifter] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 0.1f
            },

            [CreatureType.bowtorn] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false, // maybe add bone spear ability later somehow.
                Threshold = 5,
                NutritionVal = 0.1f
            },

            [CreatureType.shiver] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 0.1f
            },

            [CreatureType.bear] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = true,
                Threshold = 4, 
                NutritionVal = 5f,
                AdaptationType = typeof (BearSenses)
            },


            [CreatureType.hare] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 2,
            },

            [CreatureType.wolf] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = true,
                Threshold = 5,
                NutritionVal = 2.5f,
                AdaptationType = typeof(WolfAdaptation)

            },

            [CreatureType.fox] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 2,

            },

            [CreatureType.goat] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 3f,

            },

            [CreatureType.deer] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 3.5f,

            },

            [CreatureType.raccoon] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 0.5f,

            },

            [CreatureType.sheep] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 4.5f,

            },

            [CreatureType.chicken] = new CreatureDefinition() // Add Flight Adaptatiion -- Glide -- Investigate wingsuit thing 
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 2.5f,
            //    AdaptationType = typeof(WingedAdaptation)

            },

            [CreatureType.pig] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 8f,
 

            },

            [CreatureType.hyena] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 1.5f,

            },




            [CreatureType.unknown] = new CreatureDefinition()
            {
                IsUnknown = true
            },
        };

        public IReadOnlyDictionary<CreatureType, CreatureDefinition> CreatureLibrary => CreatureDefinitions;


        private Dictionary<Type, Func<Adaptation>> AdaptationProducer { get; } = [];
        public readonly List<Adaptation> ActiveAdaptations = [];
        public IReadOnlyDictionary<Type, Func<Adaptation>> AdaptationManager => AdaptationProducer;
        public BearSenses BearSenses { get; private set; }

        private EntityPlayer Player => entity as EntityPlayer;
        private readonly Entity entity;
        private readonly ZenithData zenithData;
        private TreeAttribute ZenithData => zenithData.Tree;
        // Always Check for Stale References -- I lost 1 day and 5 hours to this mistake.
        static public bool DebugMode => ZenithSettings.ZDebugMode;
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
                [typeof(WolfAdaptation)] = () => new WolfAdaptation(entity.World, entity, CreatureLibrary),
                [typeof(BearSenses)] = () => new BearSenses(entity.World, entity as EntityPlayer, CreatureLibrary),

            };



          //  Log($"Tree Null? {ZenithData == null}");
            InitializeAdapt();

        }

        public Adaptation CreateAdaption(CreatureType creatureType)
        {
            var type = CreatureDefinitions[creatureType].AdaptationType;

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

            var def = CreatureDefinitions[creatureType];
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

                var adaptation = CreateAdaption(creatureType);

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
            //foreach (var creature in CreatureDefinitions.Where(c => !c.Value.IsUnknown))
            //{
            //    Log($"[CA] {creature.ToString()}");
            //}
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
            var def = CreatureDefinitions[creatureType];
            foreach (var adaptation in ActiveAdaptations)
            {
                adaptation.OnAssimilate(entity, def, CreatureDefinitions);
            }
        }


       

        private void ApplyAdaptations()
        {
            foreach (var adaptation in ActiveAdaptations)
            {
                adaptation.Apply();
            }
        }
        public void SaveCAdapt()
        {

            Log($"Saving {ActiveAdaptations.Count} adaptations");

            foreach (var adadpt in ActiveAdaptations)
            {
           //     Log($"SAVE {adadpt.SourceCreature} {adadpt.GetHashCode()}");

            }
            foreach (var creature in CreatureDefinitions.Where(c => !c.Value.IsUnknown))
            {
                ZenithData.SetInt($"{creature.Key} CA-Counter", creature.Value.Counter) ;
            }

            var adaptationTree = new TreeAttribute();

            for (int i = 0; i < ActiveAdaptations.Count; i++)
            {
                adaptationTree.SetString(i.ToString(), ActiveAdaptations[i].SourceCreature.ToString());
            }



            ZenithData["adaptations"] = adaptationTree;
            entity.WatchedAttributes.MarkPathDirty("zenith");
            entity.WatchedAttributes.MarkPathDirty("adaptations");

            //Log($"Tree Null? {ZenithData == null}");


            //Log($"CreatureAdaptations instance {GetHashCode()}");
            //Log($"Active list {ActiveAdaptations.GetHashCode()}");
            //Log($"Count {ActiveAdaptations.Count}");

            //     Log($"Loading adaptations. Tree exists: {watchedZenith.GetTreeAttribute("adaptations") != null}");


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

                    var def = CreatureDefinitions[creatureType];
                    def.IsLocked = true;

                    //       Log("[LOAD] Creating adaptation");
                    var adaptation = CreateAdaption(creatureType);

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

            foreach (var creature in CreatureDefinitions.Where(c => !c.Value.IsUnknown))
            {
                creature.Value.Counter = ZenithData.GetInt($"{creature.Key} CA-Counter", 0);
            }

            //Log($"CreatureAdaptations instance {GetHashCode()}");
            //Log($"Active list {ActiveAdaptations.GetHashCode()}");
            //Log($"Count {ActiveAdaptations.Count}");
            //  Log($" adaptations Null? : {adaptationTree == null}");
        }

        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }

      
    }
}
