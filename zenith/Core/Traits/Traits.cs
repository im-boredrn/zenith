using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;
using zenith.Config;
using zenith.Core.Assimilation;
using zenith.Core.NetWork;
using zenith.Core.Progression;
using zenith.GUI;

namespace zenith.Core.Traits
{
    public class Traits
    {

        private readonly TreeAttribute watchedZenith;
        public static bool DebugMode => ZenithSettings.ZDebugMode;

        private readonly Entity entity;
        private readonly IAssimilationProvider assimilationProvider;
        private EntityPlayer Player => entity as EntityPlayer;
        private StatOutput StatOutput;
      
        public Traits(Entity entity, IAssimilationProvider assimilationProvider, StatOutput statOutput)
        {
            this.entity = entity;
            this.assimilationProvider = assimilationProvider;
            this.StatOutput = statOutput;
            var entityPlayer = Player as EntityPlayer;

            watchedZenith = (TreeAttribute)(entityPlayer.WatchedAttributes.GetTreeAttribute("zenith") ?? new TreeAttribute());
            entityPlayer.WatchedAttributes["zenith"] = watchedZenith;
        }





        private GUITotals GetGUITotals()
        {
            var gTotals = new GUITotals();
            var totals = assimilationProvider.CalculateTotals();
            float outputScale = StatOutput.OutputPercent ;

            float jumpBonus = totals.Jump + 0.49f;
            gTotals.GDamage = totals.Damage * 100f;
            gTotals.GJump = jumpBonus * outputScale;

            return gTotals  ;
        }

        public void ApplyTraits()
        {
            Log("[FLOW] ApplyTraits Called");

            var totals = assimilationProvider.CalculateTotals();
            float finalSpeed = (totals.Speed * (StatOutput.OutputPercent / 100f));
            float jumpBonus = totals.Jump + 0.49f;

            float finalJump = (jumpBonus * StatOutput.OutputPercent/100f);
            float finalDamage = (totals.Damage * 100f);

            var entityPlayer = Player as EntityPlayer;
            entityPlayer.Stats.Set("walkspeed", "zenith", totals.Speed, true); 
            entityPlayer.Stats.Set("jumpHeightMul", "zenith", finalJump, true);
            entityPlayer.Stats.Set("meleeWeaponsDamage", "zenith", totals.Damage, true);

            SaveTraits();
        }

        public void SaveTraits()
        {

            var gTotals = GetGUITotals();


            watchedZenith.SetFloat("Speed", gTotals.GSpeed);
            watchedZenith.SetFloat("JHM", gTotals.GJump);
            watchedZenith.SetFloat("Dmg", gTotals.GDamage);


           // Log($"[SAVE]  | GJump : {gTotals.GJump}\n Damage : {gTotals.GDamage}\n Speed : {gTotals.GSpeed} | NOTE: Output percent behind by 10% ");
            entity.WatchedAttributes.MarkPathDirty("zenith");

        }

        private void Log(string message)
        {
            if (!DebugMode) return;
            Player.World.Logger.Warning(message);
        }
    }
}
