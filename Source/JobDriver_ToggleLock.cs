using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Locks
{
    public class JobDriver_ToggleLock : JobDriver
    {
        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(delegate
            {
                Designation designation = Map.designationManager.DesignationOn(TargetThingA, LockUtility.DesDef);
                return designation == null;
            });
            yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(15).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            Toil toil = new Toil();
            toil.initAction = delegate
            {
                Pawn actor = toil.actor;
                ThingWithComps door = (ThingWithComps)actor.CurJob.targetA.Thing;
                this.FailOn(() => !LockUtility.GetData(door).CanChangeLocks(actor));
                LockUtility.GetData(door).CurrentState.CopyFrom(LockUtility.GetData(door).WantedState);
                SoundDefOf.FlickSwitch.PlayOneShot(new TargetInfo(door.Position, door.Map, false));
                door.Map.reachability.ClearCache();
                //actor.records.Increment(RecordDefOf.SwitchesFlicked);
                Designation designation = Map.designationManager.DesignationOn(door, LockUtility.DesDef);
                if (designation != null)
                {
                    designation.Delete();
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return toil;
        }

        public override bool TryMakePreToilReservations(bool forced)
        {
            return true;
        }
    }
}
