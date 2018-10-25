using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Locks
{
    internal class Alert_LocksCantBeChanged : Alert
    {
        public Alert_LocksCantBeChanged()
        {
            defaultLabel = "Locks_AlertLocksCantBeChanged".Translate();
            defaultExplanation = "Locks_AlertLocksCantBeChangedDefaultDesc".Translate();
        }

        private IEnumerable<Building> UnchangableDoors
        {
            get
            {
                var maps = Find.Maps;
                foreach (var map in maps)
                {
                    List<Designation> desList = map.designationManager.allDesignations;
                    for (int i = 0; i < desList.Count; i++)
                    {
                        if (desList[i].def == LockUtility.DesDef)
                        {
                            var building = desList[i].target.Thing as Building;
                            var compLock = building.GetComp<CompLock>();
                            
                            if (compLock.wantedState.Private && !compLock.AssignedPawns.Any(p => p.workSettings.WorkIsActive(DefDatabase<WorkTypeDef>.GetNamed("BasicWorker"))))
                                yield return desList[i].target.Thing as Building;
                        }
                    }
                }
            }
        }

        public override string GetExplanation()
        {
            var stringBuilder = new StringBuilder();
            foreach (var current in UnchangableDoors)
                stringBuilder.AppendLine("    " + current.Label);
            return string.Format("Locks_AlertLocksCantBeChangedDesc".Translate(), stringBuilder);
        }

        public override AlertReport GetReport()
        {
            //TODO add settings disabling
            //if (LocksPrefs.EnableAlerts)
            return AlertReport.CulpritIs(UnchangableDoors.FirstOrDefault());
            //return false;
        }
    }
}
