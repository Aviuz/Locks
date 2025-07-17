using System.Text;
using Locks.Options;
using RimWorld;
using UnityEngine;
using Verse;

namespace Locks
{
  [StaticConstructorOnStartup]
  public class DebugButtonGizmo : Command_Target
  {
    private static readonly Texture QuestionMark = ContentFinder<Texture2D>.Get("UI/Commands/ViewQuest");
    private readonly ThingWithComps parent;

    public DebugButtonGizmo(ThingWithComps parent)
    {
      this.parent = parent;
      targetingParams = TargetingParameters.ForPawns();
      action = PerformAction;
      defaultLabel = "Locks_DebugCheck".Translate();
      defaultDesc = "Locks_DebugCheck_Desc".Translate();
      icon = QuestionMark;
    }

    public override bool Visible => LocksSettings.debugButton && parent.Faction == Faction.OfPlayer;

    private void PerformAction(LocalTargetInfo target)
    {
      if (parent != null && target.Pawn != null)
      {
        var builder = new StringBuilder();
        var result = LockUtility.PawnCanOpenLogged(parent, target.Pawn, builder);
        Find.WindowStack.Add(new DebugDialog(parent, target.Pawn, builder, result));
      }
      else
      {
        Log.Error("DebugButtonGizmo: target pawn is null");
      }
    }
  }
}