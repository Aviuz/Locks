using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using RimWorld;

namespace Locks.HarmonyPatches
{
    [HarmonyPatch(typeof(Building_Door))]
    [HarmonyPatch("ExposeData")]
    public class Patch_AddLockDataToSave
    {
        private static void Postfix(Building_Door __instance)
        {
            //TODO disable utility
            if (false)
                return;

            LockUtility.GetData(__instance).ExposeData();
        }
    }
}
