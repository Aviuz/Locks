using Locks.Options;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Locks
{
  public class CompProperties_Lock : CompProperties
  {
    public CompProperties_Lock()
    {
      this.compClass = typeof(CompLock);
    }
  }

  public class CompLock : ThingComp
  {
    private static readonly string FENCE = "FenceGate";
    public override string CompInspectStringExtra()
    {
      string text = "Locks_StatePrefix".Translate() + " ";

      if (LockUtility.GetData(this.parent).CurrentState.locked)
        text += "Locks_StateLocked".Translate();
      else
        text += "Locks_StateUnlocked".Translate();
      if (LockUtility.GetData(this.parent).NeedChange)
        text += $" ({"Locks_StateChanging".Translate()})";

      return text;
    }

    public override void PostDeSpawn(Map map)
    {
      LockUtility.Remove(this.parent);
    }

    public override void PostExposeData()
    {
      LockUtility.GetData(this.parent).ExposeData();
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
      yield return new LockGizmo(this.parent);
      Command_Action command_Action = new Command_Action();
      command_Action.icon = ContentFinder<Texture2D>.Get("UI/Commands/CopySettings");
      command_Action.defaultLabel = "Locks_Copy".Translate();
      command_Action.action = delegate
      {
        SoundDefOf.Tick_High.PlayOneShotOnCamera();
        Clipboard.StoredState = LockUtility.GetData(this.parent).WantedState;
      };
      command_Action.hotKey = KeyBindingDefOf.Misc4;
      yield return command_Action;

      Command_Action command_Action2 = new Command_Action();
      command_Action2.icon = ContentFinder<Texture2D>.Get("UI/Commands/PasteSettings");
      command_Action2.defaultLabel = "CommandPasteZoneSettingsLabel".Translate();
      command_Action2.defaultDesc = "CommandPasteZoneSettingsDesc".Translate();
      command_Action2.action = delegate
      {

        SoundDefOf.Tick_High.PlayOneShotOnCamera();
        CopyUtils.SetWantedStateData(this.parent, Clipboard.StoredState.Value);

      };
      command_Action2.hotKey = KeyBindingDefOf.Misc5;
      if (Clipboard.StoredState == null)
      {
        command_Action2.Disable();
      }
      yield return command_Action2;


    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
      if ((this.parent.def.defName == FENCE || LocksSettings.alwaysPensDoor) && !respawningAfterLoad)
      {
        LockUtility.GetData(this.parent).CurrentState.pensDoor = true;
        LockUtility.GetData(this.parent).WantedState.pensDoor = true;
      }
    }
  }
}
