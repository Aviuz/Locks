using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using RimWorld;
using System.Reflection.Emit;
using Verse;
using UnityEngine;

namespace Locks.HarmonyPatches
{
    [HarmonyPatch(typeof(Building_Door), nameof(Building_Door.GetGizmos))]
    public class Patch_AddLockGizmoToDoors
    {
        static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Building __instance)
        {
            return LockUtility.AddLockGizmo(__result, __instance);
        }
    }
}
