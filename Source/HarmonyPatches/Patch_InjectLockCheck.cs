using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace Locks.HarmonyPatches
{
  [HarmonyPatch(typeof(Building_Door))]
  [HarmonyPatch("PawnCanOpen")]
  public class Patch_InjectLockCheck
  {
    private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instr)
    {
      yield return new CodeInstruction(OpCodes.Ldarg_0);
      yield return new CodeInstruction(OpCodes.Ldarg_1);
      yield return new CodeInstruction(OpCodes.Call, typeof(LockUtility).GetMethod("PawnCanOpen"));
      yield return new CodeInstruction(OpCodes.Ret);
    }
  }

  [HarmonyPatch(typeof(Building_VacBarrier), "AlwaysOpen", MethodType.Getter)]
  public class Patch_VacBarrierAlwaysOpen
  {
    private static void Postfix(ref bool __result)
    {
      __result = false;
    }
  }
}