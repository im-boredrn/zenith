using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using zenith.Core.Progression;

namespace zenith.Core.Traits
{
    public class Traits
    {

        private readonly TreeAttribute watchedZenith;

        private readonly Entity entity;
        private EntityPlayer Player => entity as EntityPlayer;

        public Traits(Entity entity)
        {
            this.entity = entity;
            var entityPlayer = Player as EntityPlayer;

            watchedZenith = (TreeAttribute)(entityPlayer.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entityPlayer.WatchedAttributes["zenith"] = watchedZenith;
        }

        static public float GetSpeedMultiplier() // Placho,der
        {
            return 3f;
        }
        static public float GetJumpHeightMultiplier( ) // Placho,der
        {

            // Im thinking config + Current Mult Level.
            return  3f;
        }

        public void ApplyTraits( )
        {
            var entityPlayer = Player as EntityPlayer;
            entityPlayer.Stats.Set("walkspeed", "zenith", GetSpeedMultiplier(), true);
            entityPlayer.Stats.Set("jumpHeightMul", "zenith", GetJumpHeightMultiplier(), true);

            



            SaveTraits();
        }

        public void SaveTraits()
        {

            watchedZenith.SetFloat("Speed", GetSpeedMultiplier());
            watchedZenith.SetFloat("JHM", GetJumpHeightMultiplier());
        }
    }
}
