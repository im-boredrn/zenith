using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

namespace zenith.Core
{
    public class ZenithData
    {

        private readonly Entity entity;

        public ZenithData(Entity entity)
        {
            this.entity = entity;
        }


        public TreeAttribute Tree
        {

            get
            {
                var tree = entity.WatchedAttributes.GetTreeAttribute("zenith");

                if (tree == null)
                {
                    tree = new TreeAttribute();
                    entity.WatchedAttributes["zenith"] = tree;
                }
                return (TreeAttribute)tree;
            }
        }

        public bool BearUnlocked
        {
            get
            {
                var adaptations = Tree.GetTreeAttribute("Adaptations");
                var bear = adaptations?.GetTreeAttribute("Bear");

                return bear?.GetBool("IsUnlocked", false) ?? false;
            }


            set
            {
                var adaptations = Tree.GetTreeAttribute("Adaptations");
                var bear = adaptations?.GetTreeAttribute("Bear");

                bear.SetBool("IsUnlocked", value);
                entity.WatchedAttributes.MarkPathDirty("zenith");

            }
        }
    }
}
