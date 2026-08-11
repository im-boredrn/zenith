//using System;
//using System.Collections.Generic;
//using System.Net.WebSockets;
//using System.Reflection;
//using System.Text;
//using Vintagestory.API.Common.Entities;
//using Vintagestory.GameContent;

//namespace zenith.Core.AdaptationsCore.AdaptationsFactory.AdaptUtil
//{
//    internal class WingedPhysicsPatch 
//    {

//        private static FieldInfo pMods;

//        public static void PatchWingedPhysics(Entity entity)
//        {
//            var behavior = entity.GetBehavior<EntityBehaviorControlledPhysics>();// find behavior
//            if (behavior == null) return;

//            if (pMods == null) // check Physics modules and find list<PModule> physicsModules
//            {
//                pMods = typeof(EntityBehaviorControlledPhysics).GetField("physicsModules", BindingFlags.NonPublic | BindingFlags.Instance);
//            }
//            if (pMods == null) return;

//            var mods = pMods.GetValue(behavior) as List<PModule>; // retrieve players physics mods

//            if (mods == null) return;

//            bool hasMod = false;

//            foreach (var mod in mods) // check for custom module
//            {
//                if (mod is WingedPlayerInAir)
//                {
//                    hasMod = true;
//                    break;
//                }
//            }

//            if (!hasMod)
//            {
//                int modPos = -1;

//                for (int i = 0; i < mods.Count; i++) // find original module position and add after
//                {
//                    if (mods[i] is PModulePlayerInAir && !(mods[i] is WingedPlayerInAir))
//                    {
//                        modPos = i + 1;
//                        break;
//                    }
//                }


//                var myMod = new WingedPlayerInAir();

//                var initialize = typeof(PModule).GetMethod("Initialize", BindingFlags.Public | BindingFlags.Instance);
//                initialize?.Invoke(myMod, new object[] { null, entity });


//                if (modPos >= 0 && modPos <= mods.Count) // check pos validity
//                {
//                    mods.Insert(modPos, myMod);
//                }
//                else
//                {
//                    mods.Add(myMod);
//                }

//                pMods.SetValue(behavior, mods);
//            }

//        }

//    }
//}
