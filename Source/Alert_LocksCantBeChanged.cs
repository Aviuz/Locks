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

    private IEnumerable<Building_Door> UnchangableDoors
    {
      get
      {
        var maps = Find.Maps;
        foreach (var map in maps)
        {
          List<Designation> desList = map.designationManager.designationsByDef[LockUtility.DesDef];
          foreach (var thing in desList.Select(d => d.target.Thing))
          {
            if (LockUtility.GetData(thing as Building_Door).WantedState.Private && !LockUtility
                  .GetData(thing as Building_Door).WantedState.ColonistDoor.AllowedPawns.Any(p =>
                    p.workSettings != null &&
                    p.workSettings.WorkIsActive(DefDatabase<WorkTypeDef>.GetNamed("BasicWorker"))))
              yield return thing as Building_Door;
          }
        }
      }
    }

    public override TaggedString GetExplanation()
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