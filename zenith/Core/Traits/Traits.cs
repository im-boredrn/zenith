using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;
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
            return 1.2f;
        }
        static public float GetJumpHeightMultiplier( ) // Placho,der
        {

            // TODO JHM ModConfig + Current Assim Level.
            return 1.3f;
        }

        public float[] GetMultValues()
        {
            return new float[]
            {
                0f,
                GetSpeedMultiplier(),
                GetJumpHeightMultiplier()
            };
        }

        public void ApplyTraits( )
        {

            float[] multVals = GetMultValues();
            var entityPlayer = Player as EntityPlayer;
            entityPlayer.Stats.Set("walkspeed", "zenith", multVals[1], true);
            entityPlayer.Stats.Set("jumpHeightMul", "zenith", multVals[2], true);

            SaveTraits();
        }

        public void SaveTraits()
        {

            watchedZenith.SetFloat("Speed", GetSpeedMultiplier());
            watchedZenith.SetFloat("JHM", GetJumpHeightMultiplier());
        }
    }
}
