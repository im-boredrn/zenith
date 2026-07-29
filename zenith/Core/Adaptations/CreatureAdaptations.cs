using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
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

        private Dictionary<Type, Func<Adaptation>> adaptationProducer { get; } = new Dictionary<Type, Func<Adaptation>>();
      

        private EntityPlayer Player => entity as EntityPlayer;
        private readonly TreeAttribute watchedZenith;
        private readonly ICoreClientAPI capi;
        static public bool DebugMode => ZenithSettings.ZDebugMode;


        public CreatureAdaptations(Entity entity, ICoreClientAPI capi) : base(entity)
        {
            this.capi = capi;

            watchedZenith = (TreeAttribute)(entity.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entity.WatchedAttributes["zenith"] = watchedZenith;

        }

        
        public Adaptation CreateAdaption(CreatureDefinition creatureDefinition)
        {

            var adaptProd = new adaptatio
            if (creatureDefinition.AdaptationType == typeof(BearAdaptation))
            {
                return new BearAdaptation(capi.World);
            }
            return null;
        }

        public void CheckAdaptation(CreatureType creatureType)
        {
         

            {
                CreatureDefinitions[creatureType].Counter += 1;

                if (CreatureDefinitions[creatureType].Counter == CreatureDefinitions[creatureType].Threshold &&
                    CreatureDefinitions[creatureType].HasAdaptation == true)
                {
                    CreatureDefinitions[creatureType].AdaptAchieved = true;
                }

                foreach (var creature in CreatureDefinitions.Where(c => !c.Value.IsUnknown ))
                {

                    Log($"[CA] {creature.ToString()}");
                    
                }
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

            foreach (var creature in CreatureDefinitions.Where(kvp => kvp.Value.Adaptation != null))
            {
                creature.Value.Adaptation.OnAssimilate(entity, def, CreatureDefinitions);
                creature.Value?.Adaptation?.GetDistance(capi);
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
