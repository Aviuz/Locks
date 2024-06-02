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
        Log.Message("Locks: Doors expanded found");
      }
      else
      {
        Log.Message("Locks: Doors expanded not found ");
      }
    }
  }
}