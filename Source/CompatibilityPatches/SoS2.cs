using HarmonyLib;
using Locks.HarmonyPatches;
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
    class SoS2
    {
        public static void Init()
        {
            var mod = LoadedModManager.RunningMods.FirstOrDefault(m => m.Name == "Save Our Ship 2");
            if (mod != null)
            {
                var doorDef = AccessTools.Method("Building_ShipAirlock:PawnCanOpen");
                if (doorDef != null)
                {
                    var harmony = new Harmony("Harmony_Locks_SoS2");
                    harmony.Patch(doorDef, transpiler: new HarmonyMethod(typeof(SoS2).GetMethod("AirLockTranspiler")));
                    Log.Message("Locks: SoS2 patched");
                }
                else
                {
                    Log.Error("Locks: mod SoS2 found but cant find airlock to patch");
                }
            }
            else
            {
                Log.Message("Locks: SoS2 not found ");
            }
        }

        public static IEnumerable<CodeInstruction> AirLockTranspiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instr)
        {
            OpCode[] codeToFind =
            {
                OpCodes.Ldarg_1,
                OpCodes.Call,
                OpCodes.Stloc_0

            };

            string[] labelToFind =
            {
                "",
                "Verse.AI.Group.Lord GetLord(Verse.Pawn)",
                ""
            };
            int step = 0;
            foreach (CodeInstruction codeInst in instr)
            {
                yield return codeInst;
                if (HPatcher.IsFragment(codeToFind, labelToFind, codeInst, ref step))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, typeof(LockUtility).GetMethod("PawnCanOpen"));
                    yield return new CodeInstruction(OpCodes.Ret);                    
                }
            }

        }

    }
}
