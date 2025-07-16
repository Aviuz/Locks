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
}