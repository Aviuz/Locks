using Locks.CompatibilityPatches;
using Locks.HarmonyPatches;
using Verse;

namespace Locks
{
  [StaticConstructorOnStartup]
  public static class Initialization
  {
    public const string VERSION = "3.1.1";
    static Initialization()
    {
      HPatcher.Init();
      ClutterStructure.Init();
      DoorsExpanded.Init();
      SoS2.Init();
      Log.Message($"Locks version: {VERSION} loaded.");
    }
  }
}