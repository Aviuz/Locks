using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using RimWorld;
using Verse;

namespace Locks.CompatibilityPatches
{
    public class DoorsExpandedPatches
    {
        private static bool CanLock(BuildableDef def)
        {
            return def.stuffCategories
                       .Count(category =>
                           category.defName == StuffCategoryDefOf.Fabric.defName 
                           || category.defName == StuffCategoryDefOf.Leathery.defName) == 0;
        }

        private static void Init()
        {
            var doorExpandedType = Type.GetType("DoorsExpanded.Building_DoorExpanded, DoorsExpanded");
            var harmony = HarmonyInstance.Create("Harmony_Locks_DoorsExpanded");
            
            harmony.Patch(
                doorExpandedType.GetMethod("PawnCanOpen", BindingFlags.Public | BindingFlags.Instance),
                new HarmonyMethod(typeof(Patch_Building_DoorExpanded_PawnCanOpen).GetMethod(nameof(Patch_Building_DoorExpanded_PawnCanOpen.Prefix)))); 
            harmony.Patch(
                doorExpandedType.GetMethod("GetGizmos", BindingFlags.Public | BindingFlags.Instance),
                null, 
                new HarmonyMethod(typeof(Patch_Building_DoorExpanded_GetGizmos).GetMethod(nameof(Patch_Building_DoorExpanded_GetGizmos.Postfix))));
            
            DefDatabase<ThingDef>.AllDefsListForReading.ForEach(def =>
            {
                if (def != null && def.thingClass == doorExpandedType)
                {
                    if (CanLock(def))
                    {
                        Initialization.AddLock(def);
                    }
                }
            });
        }
        
        [HarmonyPatch(typeof(DefGenerator), "GenerateImpliedDefs_PostResolve")]
        public class Patch_GenerateImpliedDefs_PostResolve
        {
            [HarmonyPriority(Priority.Last)]
            public static void Postfix()
            {
                if (ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name == "Doors Expanded"))
                {
                    Log.Message("Lock Mods detected Doors Expanded, loading support");
                    try
                    {
                        ((Action) (() => { Init(); }))();
                        Log.Message("Lock Mods detected Doors Expanded, loading done");
                    }
                    catch (Exception e)
                    {
                        Log.Warning("Locks Mod failed to load Doors Expanded support: " + e.Message);
                        Log.Warning(e.StackTrace);
                    }
                }
            }
        }

        public static class Patch_Building_DoorExpanded_PawnCanOpen
        {
            public static bool Prefix(Building __instance, out bool __result, Pawn p)
            {
                __result = false;
                return LockUtility.PawnCanOpen(__instance, p);
            }
        }

        public class Patch_Building_DoorExpanded_GetGizmos
        {
            public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Building __instance)
            {
                return LockUtility.AddLockGizmo(__result, __instance);
            }
        }
    }
}
