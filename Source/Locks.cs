using System;
using System.Reflection;
using Harmony;
using Verse;

namespace Locks
{
    public class Locks : Mod
    {
        public Locks(ModContentPack content) : base(content)
        {
            var harmony = HarmonyInstance.Create("Harmony_Locks");
            try
            {
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                Log.Error($"Locks Mod Exception, failed to proceed harmony patches: {e.Message}");
            }
        }
    }
}