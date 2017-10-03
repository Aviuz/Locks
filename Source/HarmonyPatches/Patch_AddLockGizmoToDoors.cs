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
    [HarmonyPatch(typeof(Building_Door))]
    [HarmonyPatch("GetGizmos")]
    public class Patch_AddLockGizmoToDoors
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instr)
        {
            OpCode[] opCodes = { OpCodes.Ret };
            String[] strings = { "" };
            int step = 0;

            foreach (var ci in instr)
            {
                if (HPatcher.IsFragment(opCodes, strings, ci, ref step))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, typeof(Patch_AddLockGizmoToDoors).GetMethod("AddLockGizmo"));
                }
                yield return ci;
            }
        }

        public static IEnumerable<Gizmo> AddLockGizmo(IEnumerable<Gizmo> collection, Building_Door door)
        {
            return collection.Add(new LockGizmo(door));
        }
    }
}
