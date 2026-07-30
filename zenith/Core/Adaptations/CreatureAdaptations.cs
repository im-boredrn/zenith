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
using CreatureType = zenith.Core.Assimilation.AssimilationCore.CreatureType;

namespace zenith.Core.Adaptations
{
    public class CreatureAdaptations : EntityBehavior
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

            [CreatureType.wolf] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = true,
                Threshold = 5,
                NutritionVal = 2.5f,
                AdaptationType = typeof(WolfAdaptation)

            },

            [CreatureType.sheep] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 3.5f,

            },

            [CreatureType.pig] = new CreatureDefinition()
            {
                Counter = 0,
                HasAdaptation = false,
                Threshold = 5,
                NutritionVal = 4.5f,
 

            },

            [CreatureType.unknown] = new CreatureDefinition()
            {
                IsUnknown = true
            },
        };

        private Dictionary<Type, Func<Adaptation>> AdaptationProducer { get; } = new();
        private readonly List<Adaptation> ActiveAdaptations;

        private EntityPlayer Player => entity as EntityPlayer;
        private readonly TreeAttribute watchedZenith;
        static public bool DebugMode => ZenithSettings.ZDebugMode;


        public CreatureAdaptations(Entity entity) : base(entity)
        {
            // this.core = coreAPI;
            if (Player == null)
            {
                throw new Exception("CreatureAdaptations attached to non-player entity");
            }


            AdaptationProducer = new Dictionary<Type, Func<Adaptation>>()
            {
                [typeof(WolfAdaptation)] = () => new WolfAdaptation(entity.World,entity),
                [typeof(BearSenses)] = () => new BearSenses(entity.World, entity as EntityPlayer)


            };

            ActiveAdaptations = new List<Adaptation>();
            
            watchedZenith = (TreeAttribute)(entity.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entity.WatchedAttributes["zenith"] = watchedZenith;

            LoadCAdapt();


        }

        public Adaptation CreateAdaption(CreatureType creatureType)
        {
            var type = CreatureDefinitions[creatureType].AdaptationType;

            if (type != null && AdaptationProducer.TryGetValue(type, out var factory))
            {
                return factory();
            }

            return null;
        }

        public void CheckAdaptation(CreatureType creatureType)
        {

            var def = CreatureDefinitions[creatureType];
            var sapi = entity.World.Api as ICoreServerAPI;

            def.Counter += 1;

            if (def.Counter > def.Threshold)
            {
                def.Counter = def.Threshold;
            }


            if (def.Counter == def.Threshold &&
                def.HasAdaptation )
            {
                var adaptation = CreateAdaption(creatureType);

                sapi.SendMessage(Player.Player, GlobalConstants.AllChatGroups, $"{creatureType.ToString()} Adaptation Successfully Assimilated", EnumChatType.Notification);



                if (adaptation != null)
                {
                    ActiveAdaptations.Add(adaptation);

                    
                }

            }

            SaveCAdapt();
            foreach (var creature in CreatureDefinitions.Where(c => !c.Value.IsUnknown))
            {
                
                Log($"[CA] {creature.ToString()}");

            }
        }

        public void Tick(float dt)
        {
            foreach (var adaptation in ActiveAdaptations)
            {
                adaptation.Tick(dt);
            }
        }

   
        public void AssimilateLink(CreatureType creatureType) 
        {
            Log("[CA-FLOW] AssimilateLink Called");
            var def = CreatureDefinitions[creatureType];
            foreach (var adaptation in ActiveAdaptations)
            {
                adaptation.OnAssimilate(entity, def, CreatureDefinitions);
            }
        }
        public void SaveCAdapt()
        {
            foreach (var creature in CreatureDefinitions.Where(c => !c.Value.IsUnknown))
            {
                watchedZenith.SetInt($"{creature.Key} CA-Counter", creature.Value.Counter) ;
            }

            var adaptationTree = new TreeAttribute();

            for (int i = 0; i < ActiveAdaptations.Count; i++)
            {
                adaptationTree.SetString(i.ToString(), ActiveAdaptations[i].SourceCreature.ToString());
            }

            watchedZenith["adaptations"] = adaptationTree;
        }

        public void LoadCAdapt()
        {

            var adaptationTree = watchedZenith.GetTreeAttribute("adaptations");

            if (adaptationTree != null)
            {
                foreach (var key in adaptationTree)
                {
                    var creatureType = Enum.Parse<CreatureType>(adaptationTree.GetString(key.Key));

                    var def = CreatureDefinitions[creatureType];

                    var adaptation = CreateAdaption(creatureType); 

                    if (adaptation != null)
                    {
                        ActiveAdaptations.Add(adaptation);
                    }
                }
            }

            foreach (var creature in CreatureDefinitions.Where(c => !c.Value.IsUnknown))
            {
                creature.Value.Counter = watchedZenith.GetInt($"{creature.Key} CA-Counter", 0);
            }
        }

        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }

        public override string PropertyName()
        {
            return "CreatureAdaptations";
        }
    }
}
