using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Locks.CompatibilityPatches
{
    class DoorsExpanded
    {
        public static void Init()
        {
            var mod = LoadedModManager.RunningMods.FirstOrDefault(m => m.Name == "Doors Expanded (Dev)" || m.Name == "Doors Expanded");
            if (mod != null)
            {
                var doorDef = AccessTools.Method("DoorsExpanded.Building_DoorExpanded:PawnCanOpen");
                if (doorDef != null)
                {
                    var harmony = new Harmony("Harmony_Locks_DoorsExpanded");
                    harmony.Patch(doorDef, transpiler: new HarmonyMethod(typeof(DoorsExpanded).GetMethod("DoorsExpandedTranspiler")));
                    Log.Message("Locks: Doors expanded patched");
                }
                else
                {
                    Log.Error("Locks: found doors expanded but can't find door expanded building def");
                }
            }
            else
            {
                Log.Message("Locks: Doors expanded not found ");
            }
        }

        public static IEnumerable<CodeInstruction> DoorsExpandedTranspiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instr)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Call, typeof(LockUtility).GetMethod("PawnCanOpen"));
            yield return new CodeInstruction(OpCodes.Ret);
        }

    }
}
