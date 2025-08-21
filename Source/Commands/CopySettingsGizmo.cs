using Locks.Options;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Locks.Commands
{
  public class CopySettingsGizmo : Command_Action
  {
    private readonly ThingWithComps parent;

    public CopySettingsGizmo(ThingWithComps parent)
    {
      this.parent = parent;
      icon = ContentFinder<Texture2D>.Get("UI/Commands/CopySettings");
      defaultLabel = "CommandCopyZoneSettingsLabel".Translate();
      action = ClickAction;
      hotKey = KeyBindingDefOf.Misc4;
    }

    public override bool Visible => LockUtility.ShouldGizmoBeVisible(parent);

    private void ClickAction()
    {
      SoundDefOf.Tick_High.PlayOneShotOnCamera();
      Clipboard.StoredState = LockUtility.GetData(parent).WantedState;
    }
  }
}