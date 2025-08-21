using System.Collections.Generic;
using Locks.Commands;
using Locks.Options;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Locks
{
  public class CompProperties_Lock : CompProperties
  {
    public CompProperties_Lock()
    {
      compClass = typeof(CompLock);
    }
  }

  public class CompLock : ThingComp
  {
    private LockData lockData = new LockData();

    public LockData LockData
    {
      get => lockData ?? (lockData = new LockData());
      set => lockData = value;
    }

    public override string CompInspectStringExtra()
    {
      if (parent.Faction != Faction.OfPlayer)
      {
        return "";
      }

      string text = "Locks_StatePrefix".Translate() + " ";

      if (LockData.CurrentState.Locked)
        text += "Locks_StateLocked".Translate();
      else
        text += "Locks_StateUnlocked".Translate();
      if (LockData.NeedChange)
        text += $" ({"Locks_StateChanging".Translate()})";

      return text;
    }

    public override void PostExposeData()
    {
      LockData.ExposeData();
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
      yield return new LockGizmo(parent);
      yield return new DebugButtonGizmo(parent);
      yield return new ResetButtonGizmo(parent);
      yield return new CopySettingsGizmo(parent);
      var pasteSettingsGizmo = new PasteSettingsGizmo(parent);
      if (Clipboard.StoredState == null)
      {
        pasteSettingsGizmo.Disable();
      }
      yield return pasteSettingsGizmo;
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
      if ((LocksDefsOf.Locks_AllFenceDoors.fenceGatesDefNames.Contains(parent.def.defName) ||
           LocksSettings.alwaysPensDoor) && !respawningAfterLoad)
      {
        LockData.CurrentState.AnimalDoor.PensDoor = true;
        LockData.WantedState.AnimalDoor.PensDoor = true;
      }
    }
  }
}