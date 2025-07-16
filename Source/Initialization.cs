using Locks.CompatibilityPatches;
using Locks.HarmonyPatches;
using Verse;

namespace Locks
{
  [StaticConstructorOnStartup]
  public static class Initialization
  {
    static Initialization()
    {
      HPatcher.Init();
      ClutterStructure.Init();
      DoorsExpanded.Init();
      SoS2.Init();
    }
  }
}