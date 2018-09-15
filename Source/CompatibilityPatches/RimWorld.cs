using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Locks.CompatibilityPatches
{
    public class RimWorld
    {
        public static void Init()
        {
            StringBuilder logMessage = new StringBuilder();
            logMessage.Append("[Locks] Adding locks to: ");
            DefDatabase<ThingDef>.AllDefsListForReading
                .Where(def => def.thingClass == typeof(Building_Door))
                .ToList()
                .ForEach(def =>
                {
                    logMessage.Append(" " + def.defName);
                    if (def.inspectorTabs == null)
                        def.inspectorTabs = new List<System.Type>();
                    def.inspectorTabs.Add(typeof(ITab_Lock));

                    if (def.comps == null)
                        def.comps = new List<CompProperties>();
                    def.comps.Add(new CompProperties_Lock());

                    def.ResolveReferences();
                });
            Log.Message(logMessage.ToString().Trim());
        }
    }
}