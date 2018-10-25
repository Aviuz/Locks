using Harmony;
using RimWorld;
using Verse;

namespace Locks.HarmonyPatches
{
    [HarmonyPatch(typeof(Building_Door), nameof(Building_Door.PawnCanOpen))]
    public class Patch_InjectLockCheck
    {
        private static bool Prefix(Building_Door __instance, out bool __result, Pawn p)
        {
            __result = false;
            var compLock = __instance.GetComp<CompLock>();
            if (compLock == null) return true;
            __result = LockUtility.PawnCanOpen(__instance, p);
            return false;
        }
    }
}
