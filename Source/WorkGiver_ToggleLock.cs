using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using Verse;
using Verse.AI;

namespace Locks
{
    public class WorkGiver_ToggleLock : WorkGiver_Scanner
    {
        [DebuggerHidden]
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            var desList = pawn.Map.designationManager.designationsByDef[LockUtility.DesDef];
            for (var i = 0; i < desList.Count; i++)
            {
                yield return desList[i].target.Thing;
            }
        }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
      var door = (ThingWithComps)t;
      if (!LockUtility.GetData(door).CanChangeLocks(pawn))
      {
        JobFailReason.Is("Locks_FailOnWrongUser".Translate(pawn));
        return false;
      }

      return pawn.Map.designationManager.DesignationOn(t, LockUtility.DesDef) != null &&
             pawn.CanReserveAndReach(t, PathEndMode.Touch, pawn.NormalMaxDanger(), 1, -1, null, forced);
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
      var door = (ThingWithComps)t;
      if (!LockUtility.GetData(door).CanChangeLocks(pawn))
      {
        JobFailReason.Is("Locks_FailOnWrongUser".Translate(pawn));
        return null;
      }

      return new Job(LockUtility.JobDef, t);
    }
  }
}