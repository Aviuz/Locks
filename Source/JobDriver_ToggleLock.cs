using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
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
        var designation = Map.designationManager.DesignationOn(TargetThingA, LockUtility.DesDef);
        return designation == null;
      });
      yield return Toils_Reserve.Reserve(TargetIndex.A);
      yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
      yield return Toils_General.Wait(15).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
      var toil = new Toil();
      toil.initAction = delegate
      {
        var actor = toil.actor;
        var door = (ThingWithComps)actor.CurJob.targetA.Thing;
        this.FailOn(() => !LockUtility.GetData(door).CanChangeLocks(actor));
        LockUtility.GetData(door).CurrentState.CopyFrom(LockUtility.GetData(door).WantedState);
        SoundDefOf.FlickSwitch.PlayOneShot(new TargetInfo(door.Position, door.Map));
        door.Map.reachability.ClearCache();
        //actor.records.Increment(RecordDefOf.SwitchesFlicked);
        var designation = Map.designationManager.DesignationOn(door, LockUtility.DesDef);
        designation?.Delete();
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