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
 

        public virtual void Save(TreeAttribute tree)
        {

            var adaptations = tree.GetOrAddTreeAttribute("Adaptations");


            foreach (var property in GetType().GetProperties())
            {

                var adaptationtree = adaptations.GetOrAddTreeAttribute(GetType().Name);

                var value = property.GetValue(this);
                string key = $"{GetType().Name} {property.Name}";

                if (property.PropertyType == typeof(bool))
                {
                    adaptationtree.SetBool(key, (bool)value);
                }

                if (property.PropertyType == typeof(int))
                {
                    adaptationtree.SetInt(key, (int)value);

                }


                if (property.PropertyType == typeof(float))
                {
                    adaptationtree.SetFloat(key, (float)value);

                }

            }
        }

        public virtual void Load(TreeAttribute tree)
        {
            var adaptations = tree.GetTreeAttribute("Adaptations");

            if (adaptations == null) return;
            foreach (var property in GetType().GetProperties())
            {

                var adaptationtree = adaptations.GetTreeAttribute(GetType().Name);

                if (adaptationtree == null) return;

                var value = property.GetValue(this);
                string key = $"{GetType().Name} {property.Name}";


                if (property.PropertyType == typeof(bool))
                {
                    property.SetValue(this, adaptationtree.GetBool(key, false));
                }

                if (property.PropertyType == typeof(int))
                {
                    property.SetValue(this, adaptationtree.GetInt(key, (int)value));  
                    

                }

                if (property.PropertyType == typeof(float))
                {
                    property.SetValue(this, adaptationtree.GetFloat(key, (float)value));


                }
            }
        }

    }
}
