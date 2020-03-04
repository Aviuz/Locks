using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using RimWorld;

namespace Locks.HarmonyPatches
{
    [HarmonyPatch(typeof(Building_Door))]
    [HarmonyPatch("DeSpawn")]
    public class Patch_RemoveLockData
    {
        private static void Postfix(Building_Door __instance)
        {
            LockUtility.Remove(__instance);
        }
    }
}
