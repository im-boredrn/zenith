using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core.Assimilation;
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


        //static public float GetSpeedMultiplier() // Possibly a fox, or wolf etc
        //{
        //    return 1.2f;
        //}
        // public float GetJumpHeightMultiplier( ) // Maybe A hare
        //{

        //    // TODO JHM ModConfig + Current Assim Level.
        //    float finalVal = ZenithSettings.ZStageJumpHeightMultipiler; // get total speed bonus

        //    // final val + TraitsTotal SpeedBonus
        //    return finalVal;
        //}

        //static public float GetDamageMultiplier() // hostile entities like bear, drifter etc.
        //{

        //    return 1f;
        //}


        //public float[] GetMultValues()
        //{
        //    return new float[]
        //    {
        //        0f,
        //        GetSpeedMultiplier(),//1
        //        GetJumpHeightMultiplier(),//2
        //        GetDamageMultiplier()
        //    };
        //}

        public void ApplyTraits()
        {

            var totals = assimilationProvider.CalculateTotals();
            var entityPlayer = Player as EntityPlayer;
            entityPlayer.Stats.Set("walkspeed", "zenith", totals.Speed, true); 
            entityPlayer.Stats.Set("jumpHeightMul", "zenith", totals.Jump, true);
            entityPlayer.Stats.Set("meleeWeaponsDamage", "zenith", totals.Damage, true);

            SaveTraits();
        }

        public void SaveTraits()
        {
            var totals = assimilationProvider.CalculateTotals();

            float speedBonus = (totals.Speed * 100f);

            float  jumpDamage = (totals.Jump * 100f);

            float damageBonus = (totals.Damage * 100f);

//            float mineSpeedBonus = (totals.Mine * 100f);
            // Work on Mining speed it looks outta place in the GUI

             



            watchedZenith.SetFloat("Speed", speedBonus);
            watchedZenith.SetFloat("JHM", jumpDamage);
            watchedZenith.SetFloat("Dmg", damageBonus);
            //watchedZenith.SetFloat("MSM", mineSpeedBonus);

           // No need to load just saving for GUI retrieval
              
        }
    }
}
