using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Locks
{
    [StaticConstructorOnStartup]
    public static class Initialization
    {
        static Initialization()
        {
            HarmonyPatches.HPatcher.Init();
            CompatibilityPatches.RimWorld.Init();
            CompatibilityPatches.ClutterStructure.Init();
        }
    }
}
