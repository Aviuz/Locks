using RimWorld;
using Verse;

namespace Locks
{
    [StaticConstructorOnStartup]
    public static class Initialization
    {
        static Initialization()
        {
            HarmonyPatches.HPatcher.Init();
        }
    }
}
