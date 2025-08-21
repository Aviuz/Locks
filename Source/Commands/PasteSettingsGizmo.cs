using Locks.Options;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Locks.Commands
{
  public class PasteSettingsGizmo : Command_Action
  {
    private readonly ThingWithComps parent;

    public PasteSettingsGizmo(ThingWithComps parent)
    {
      this.parent = parent;
      icon = ContentFinder<Texture2D>.Get("UI/Commands/PasteSettings");
      defaultLabel = "CommandPasteZoneSettingsLabel".Translate();
      action = ClickAction;
      hotKey = KeyBindingDefOf.Misc5;
    }

    public override bool Visible => LockUtility.ShouldGizmoBeVisible(parent);

    private void ClickAction()
    {
      SoundDefOf.Tick_High.PlayOneShotOnCamera();
      CopyUtils.SetWantedStateData(parent, Clipboard.StoredState.Value);
    }
  }
}