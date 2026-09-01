using HarmonyLib;
using Locks.Options;
using RimWorld;

namespace Locks.HarmonyPatches
{
  [HarmonyPatch(typeof(LordJob_PrisonBreak), "CanOpenAnyDoor")]
  public class PrisonerEscapePatch
  {
    private static bool Postfix(bool __result)
    {
      return !LocksSettings.prisonerBreakRespectsLock;
    }
  }

  [HarmonyPatch(typeof(LordJob_SlaveRebellion), "CanOpenAnyDoor")]
  public class SlaveRebelionPatch
  {
    private static bool Postfix(bool __result)
    {
      return !LocksSettings.revoltRespectsLocks;
    }
  }

  [HarmonyPatch(typeof(LordJob_TradeWithColony), "CanOpenAnyDoor")]
  public class AnimalCaravanaRespectPatch
  {
    private static bool Postfix(bool __result)
    {
      return false;
    }
  }
}