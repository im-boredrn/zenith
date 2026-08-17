using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Datastructures;
using zenith.Core.Definitions;
using static zenith.Core.Definitions.CreatureDefinition;

namespace zenith.Core.AdaptationsCore.AdaptationData
{
    public abstract class AdaptationState 
    {
        public  virtual int Counter { get; set; }
        public virtual int BlockLVL { get; set; }
        public virtual bool IsUnlocked { get; set; }
        public virtual int EvolutionStage { get; set; } = 1;
        public virtual int EvolutionRequirement { get; set; }

        public virtual void Save(TreeAttribute tree)
        {
            foreach (var property in GetType().GetProperties())
            {
                var value = property.GetValue(this);
                string key = $"{GetType().Name} {property.Name}";

                if (property.PropertyType == typeof(bool))
                {
                    tree.SetBool(key, (bool)value);
                }

                if (property.PropertyType == typeof(int))
                {
                    tree.SetInt(key, (int)value);

                }


                if (property.PropertyType == typeof(float))
                {
                    tree.SetFloat(key, (float)value);

                }

            }
        }

        public virtual void Load(TreeAttribute tree)
        {

            foreach (var property in GetType().GetProperties())
            {
                var value = property.GetValue(this);
                string key = $"{GetType().Name} {property.Name}";


                if (property.PropertyType == typeof(bool))
                {
                    property.SetValue(this, tree.GetBool(key, false));
                }

                if (property.PropertyType == typeof(int))
                {
                    property.SetValue(this, tree.GetInt(key, (int)value));  
                    

                }

                if (property.PropertyType == typeof(float))
                {
                    property.SetValue(this, tree.GetFloat(key, (float)value));


                }
            }
        }

    }
}
