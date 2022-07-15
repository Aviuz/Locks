using HarmonyLib;
using Locks.Options;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locks.HarmonyPatches
{
    [HarmonyPatch(typeof(LordJob_PrisonBreak), "CanOpenAnyDoor")]
    public class PrisonerEscapePatch
    {
        static bool Postfix(bool __result)
        {
            return !LocksSettings.prisonerBreakRespectsLock;
        }
    }

    [HarmonyPatch(typeof(LordJob_SlaveRebellion), "CanOpenAnyDoor")]
    public class SlaveRebelionPatch
    {
        static bool Postfix(bool __result)
        {
            return !LocksSettings.revoltRespectsLocks;
        }
    }
}
