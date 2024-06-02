using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Locks
{
    [StaticConstructorOnStartup]
    public static class Initialization
    {
        static Initialization()
        {
            HarmonyPatches.HPatcher.Init();
            CompatibilityPatches.ClutterStructure.Init();
            CompatibilityPatches.SoS2.Init();
        }
    }
}
