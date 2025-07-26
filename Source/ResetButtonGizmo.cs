using Locks.Options;
using RimWorld;
using UnityEngine;
using Verse;

namespace Locks
{
  [StaticConstructorOnStartup]
  public class ResetButtonGizmo : Command
  {
    private static readonly Texture QuestionMark = ContentFinder<Texture2D>.Get("UI/Commands/ViewQuest");
    private readonly ThingWithComps parent;

    public ResetButtonGizmo(ThingWithComps parent)
    {
      this.parent = parent;
      defaultLabel = "Locks_ForceReset".Translate();
      defaultDesc = "Locks_ForceReset_Desc".Translate();
      icon = QuestionMark;
    }

    public override bool Visible => LocksSettings.debugButton && parent.Faction == Faction.OfPlayer;

    public override void ProcessInput(Event ev)
    {
      base.ProcessInput(ev);
      if (ev.button == 0)
      {
        LockUtility.ResetData(parent);
      }
    }
  }
}