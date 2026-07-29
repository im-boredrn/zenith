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
                AdaptationType = typeof (BearAdaptation)
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

        private Dictionary<Type, Func<Adaptation>> AdaptationProducer { get; } = new Dictionary<Type, Func<Adaptation>>();
        private readonly List<Adaptation> ActiveAdaptations;

        private EntityPlayer Player => entity as EntityPlayer;
        private readonly TreeAttribute watchedZenith;
        static public bool DebugMode => ZenithSettings.ZDebugMode;


        public CreatureAdaptations(Entity entity) : base(entity)
        {
           // this.core = coreAPI;

            AdaptationProducer = new Dictionary<Type, Func<Adaptation>>()
            {
                [typeof(WolfAdaptation)] = () => new WolfAdaptation(entity.World,entity),
                [typeof(BearAdaptation)] = () => new BearAdaptation(entity.World, entity as EntityPlayer)


            };

            ActiveAdaptations = new List<Adaptation>()
            {

            };
            watchedZenith = (TreeAttribute)(entity.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entity.WatchedAttributes["zenith"] = watchedZenith;

        }

        
        public Adaptation CreateAdaption(CreatureDefinition creatureDefinition)
        {

            if (AdaptationProducer.TryGetValue(creatureDefinition.AdaptationType, out var factory))
            {
                return  factory( );
            }
            return null;
        }

        public void CheckAdaptation(CreatureType creatureType)
        {

            var def = CreatureDefinitions[creatureType];
            var sapi = entity.World.Api as ICoreServerAPI;

            def.Counter += 1;

            if (def.Counter == def.Threshold &&
                def.HasAdaptation && !def.AdaptAchieved == true)
            {
                def.AdaptAchieved = true;
                var adaptation = CreateAdaption(def);

                sapi.SendMessage(Player.Player, GlobalConstants.AllChatGroups, $"{creatureType.ToString()} Adaptation Successfully Assimilated", EnumChatType.Notification);



                if (adaptation != null)
                {
                    ActiveAdaptations.Add(adaptation);
                }

            }



            foreach (var creature in CreatureDefinitions.Where(c => !c.Value.IsUnknown))
            {

                Log($"[CA] {creature.ToString()}");

            }

        }

        public bool AdaptAchieved(CreatureType creatureType)
        {
            return CreatureDefinitions[creatureType].AdaptAchieved;
        }

        //public void GiveAdaptation(CreatureType creatureType)
        //{
        //    if (AdaptAchieved(creatureType))
        //    {
        //        CreatureDefinitions[creatureType].Adaptation.(Player);
        //    }
        //}
        public void AssimilateLink(CreatureType creatureType) 
        {
            Log("[CA-FLOW] AssimilateLink Called");
            var def = CreatureDefinitions[creatureType];
            float dt = 30;
            foreach (var adaptation in ActiveAdaptations)
            {
                adaptation.Tick(dt);
                adaptation.OnAssimilate(entity, def, CreatureDefinitions);
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
