using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core.Progression;

namespace zenith.Core.Traits
{
    public class Traits
    {

        private readonly TreeAttribute watchedZenith;

        private readonly Entity entity;
        private readonly IAssimilationProvider assimilationProvider;
        private EntityPlayer Player => entity as EntityPlayer;

      
        public Traits(Entity entity, IAssimilationProvider assimilationProvider)
        {
            this.entity = entity;
            this.assimilationProvider = assimilationProvider;
            var entityPlayer = Player as EntityPlayer;

            watchedZenith = (TreeAttribute)(entityPlayer.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entityPlayer.WatchedAttributes["zenith"] = watchedZenith;
        }

        static public float GetSpeedMultiplier() // Possibly a fox, or wolf etc
        {
            return 1.2f;
        }
         public float GetJumpHeightMultiplier( ) // Maybe A bunny
        {

            // TODO JHM ModConfig + Current Assim Level.
            float finalVal = ZenithSettings.ZStageJumpHeightMultipiler + assimilationProvider.GetAssimStage();


            return finalVal;
        }

        static public float GetDamageMultiplier() // hostile entities like bear, drifter etc.
        {

            return 1f;
        }


        public float[] GetMultValues()
        {
            return new float[]
            {
                0f,
                GetSpeedMultiplier(),//1
                GetJumpHeightMultiplier()//2
            };
        }

        public void ApplyTraits( )
        {

            float[] traitVals = GetMultValues();
            var entityPlayer = Player as EntityPlayer;
            entityPlayer.Stats.Set("walkspeed", "zenith", traitVals[1], true); // Eventually values will come from AssimDef.
            entityPlayer.Stats.Set("jumpHeightMul", "zenith", traitVals[2], true);
            entityPlayer.Stats.Set("meleeWeaponsDamage", "zenith", traitVals[3], true);//Assim

            SaveTraits();
        }

        public void SaveTraits()
        {
            float[] traitVals = GetMultValues();

            watchedZenith.SetFloat("Speed", traitVals[1]);
            watchedZenith.SetFloat("JHM", traitVals[2]);
        }
    }
}
