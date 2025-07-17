using System.Linq;
using Verse;

namespace Locks.CompatibilityPatches
{
  class DoorsExpanded
  {
    public static void Init()
    {
      var mod = LoadedModManager.RunningMods.FirstOrDefault(m =>
        m.Name == "Doors Expanded (Dev)" || m.Name == "Doors Expanded");
      if (mod != null)
      {
        Log.Message("Locks: Doors expanded found");
      }
      else
      {
        Log.Message("Locks: Doors expanded not found ");
      }
    }
  }
}