using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace Locks.CompatibilityPatches
{
    public class ClutterStructure
    {
        public static void Init()
        {
            var doorDef = DefDatabase<ThingDef>.GetNamed("ReinforcedDoor", false);
            if (doorDef != null)
            {
                var harmony = HarmonyInstance.Create("Harmony_Locks_ClutterStructure");
                harmony.Patch(
                doorDef.thingClass.GetMethod("PawnCanOpen"),
                new HarmonyMethod(null), new HarmonyMethod(null), new HarmonyMethod(typeof(ClutterStructure).GetMethod("DisableClutterTranspiler")));
            }
        }

        public static IEnumerable<CodeInstruction> DisableClutterTranspiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instr)
        {
            var label = gen.DefineLabel();
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Call, typeof(ClutterStructure).GetMethod("CanSurpass"));
            yield return new CodeInstruction(OpCodes.Brfalse, label);
            yield return new CodeInstruction(OpCodes.Ldc_I4_1);
            yield return new CodeInstruction(OpCodes.Ret);

            bool first = true;
            foreach (var ci in instr)
            {
                if (first)
                {
                    ci.labels.Add(label);
                    first = false;
                }
                yield return ci;
            }
        }

        public static bool CanSurpass(Building_Door door, Pawn p)
        {
            return LockUtility.GetData(door).CurrentState.locked == false && p.RaceProps != null && p.RaceProps.intelligence >= Intelligence.Humanlike;
        }
    }
}
