﻿using System.Collections.Generic;
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
    public override string CompInspectStringExtra()
    {
      if (parent.Faction != Faction.OfPlayer)
      {
        return "";
      }
      string text = "Locks_StatePrefix".Translate() + " ";

      if (LockUtility.GetData(parent).CurrentState.Locked)
        text += "Locks_StateLocked".Translate();
      else
        text += "Locks_StateUnlocked".Translate();
      if (LockUtility.GetData(parent).NeedChange)
        text += $" ({"Locks_StateChanging".Translate()})";

      return text;
    }

    public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
    {
      LockUtility.Remove(parent);
    }

    public override void PostExposeData()
    {
      LockUtility.GetData(parent).ExposeData();
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
      yield return new LockGizmo(parent);
      yield return new DebugButtonGizmo(parent);
      var command_Action = new Command_Action();
      command_Action.icon = ContentFinder<Texture2D>.Get("UI/Commands/CopySettings");
      command_Action.defaultLabel = "CommandCopyZoneSettingsLabel".Translate();
      command_Action.action = delegate
      {
        SoundDefOf.Tick_High.PlayOneShotOnCamera();
        Clipboard.StoredState = LockUtility.GetData(parent).WantedState;
      };
      command_Action.hotKey = KeyBindingDefOf.Misc4;
      yield return command_Action;

      var command_Action2 = new Command_Action();
      command_Action2.icon = ContentFinder<Texture2D>.Get("UI/Commands/PasteSettings");
      command_Action2.defaultLabel = "CommandPasteZoneSettingsLabel".Translate();
      command_Action2.action = delegate
      {
        SoundDefOf.Tick_High.PlayOneShotOnCamera();
        CopyUtils.SetWantedStateData(parent, Clipboard.StoredState.Value);
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
      if ((LocksDefsOf.Locks_AllFenceDoors.fenceGatesDefNames.Contains(parent.def.defName) ||
           LocksSettings.alwaysPensDoor) && !respawningAfterLoad)
      {
        LockUtility.GetData(parent).CurrentState.AnimalDoor.PensDoor = true;
        LockUtility.GetData(parent).WantedState.AnimalDoor.PensDoor = true;
      }
    }
  }
}