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
    }
}
