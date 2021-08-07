using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace Locks
{
    public class WorkGiver_ToggleLock : WorkGiver_Scanner
    {
        [DebuggerHidden]
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            List<Designation> desList = pawn.Map.designationManager.allDesignations;
            for (int i = 0; i < desList.Count; i++)
            {
                if (desList[i].def == LockUtility.DesDef)
                {
                    yield return desList[i].target.Thing;
                }
            }
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            ThingWithComps door = (ThingWithComps)t;
            if (!LockUtility.GetData(door).CanChangeLocks(pawn))
            {
                JobFailReason.Is("Locks_FailOnWrongUser".Translate(pawn));
                return false;
            }
            return pawn.Map.designationManager.DesignationOn(t, LockUtility.DesDef) != null && pawn.CanReserveAndReach(t, PathEndMode.Touch, pawn.NormalMaxDanger(), 1, -1, null, forced);
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {

            ThingWithComps door = (ThingWithComps)t;
            if (!LockUtility.GetData(door).CanChangeLocks(pawn))
            {
                JobFailReason.Is("Locks_FailOnWrongUser".Translate(pawn));
                return null;
            }
            return new Job(LockUtility.JobDef, t);
        }
    }
}
