using System;
using Multiplayer.API;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Locks
{
  class LockGizmo : Command
  {
    private readonly Func<bool> isActive;

    private readonly Texture2D lockTexture;
    private readonly Texture2D unlockTexture;
    public ThingWithComps parent;

    public LockGizmo(ThingWithComps door)
    {
      parent = door;
      defaultLabel = "Locks_Label".Translate();
      defaultDesc = "Locks_Description".Translate();
      lockTexture = ContentFinder<Texture2D>.Get("lock", false);
      unlockTexture = ContentFinder<Texture2D>.Get("unlock", false);
      isActive = () => LockUtility.GetData(parent).WantedState.Locked;
    }

    public override bool Visible => parent.Faction == Faction.OfPlayer;

    public override void ProcessInput(Event ev)
    {
      switch (ev.button)
      {
        case 0:
          InvertLockDesignation();
          break;
        case 1:
          Find.WindowStack.Add(new LockOptionsDialog(parent));
          break;
      }
    }

    [SyncMethod]
    private void InvertLockDesignation()
    {
      SoundDefOf.Click.PlayOneShotOnCamera();
      LockUtility.GetData(parent).WantedState.Locked = !LockUtility.GetData(parent).WantedState.Locked;
      LockUtility.UpdateLockDesignation(parent);
    }

    public override void DrawIcon(Rect rect, Material buttonMat, GizmoRenderParms parms)
    {
      icon = (LockUtility.GetData(parent).WantedState.Locked ? lockTexture : unlockTexture) ?? BaseContent.BadTex;
      base.DrawIcon(rect, buttonMat, parms);
    }

    public override bool InheritInteractionsFrom(Gizmo other)
    {
      if (other is LockGizmo lockGizmo)
      {
        return lockGizmo.isActive() == isActive();
      }

      return false;
    }
  }
}