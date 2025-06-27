using System;
using System.IO;
using System.Text;
using Locks.Debug;
using Locks.Options;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace Locks
{
  struct PawnDebug : IMarkdown
  {
    private String FullnameNotColored;

    public PawnDebug(Pawn targetPawn)
    {
      Fullname = $"{targetPawn.NameFullColored}({targetPawn})";
      FullnameNotColored = $"{targetPawn.Name}({targetPawn})";
      KindDef = $"{targetPawn.kindDef?.defName}";
      Faction = $"{targetPawn.Faction}";
      Intelligence = $"{targetPawn.RaceProps?.intelligence}";
      IsColonist = $"{targetPawn.IsColonist}";
      IsSlave = $"{targetPawn.IsSlave}";
      IsPrisoner = $"{targetPawn.IsPrisoner}";
      HostFaction = $"{targetPawn.HostFaction}";
      Size = $"{targetPawn.BodySize}";
      BioAge = $"{targetPawn.ageTracker?.AgeBiologicalYears}";
      FenceBlocked = $"{targetPawn.RaceProps?.FenceBlocked}";
      IsRoped = $"{targetPawn.roping.IsRopedByPawn}";
      LordJob = $"{targetPawn.GetLord()?.LordJob?.GetType().Name}";
      LordJobOpenAnyDoor = $"{targetPawn.GetLord()?.LordJob?.CanOpenAnyDoor(targetPawn) ?? false}";
      IsMutant = $"{targetPawn.IsMutant}";
      MutantDef = $"{targetPawn.mutant?.Def.defName}";
      MutantOpenAnyDoor = $"{targetPawn.mutant?.Def.canOpenAnyDoor}";
    }

    public string Fullname { get; }
    public string KindDef { get; }
    public string Faction { get; }
    public string Intelligence { get; }
    public string IsColonist { get; }
    public string IsSlave { get; }
    public string IsPrisoner { get; }
    public string HostFaction { get; }
    public string Size { get; }
    public string BioAge { get; }
    public string FenceBlocked { get; }
    public string IsRoped { get; }
    public string LordJob { get; }
    public string LordJobOpenAnyDoor { get; }
    public string IsMutant { get; }
    public string MutantDef { get; }
    public string MutantOpenAnyDoor { get; }

    public StringBuilder ToMarkdown()
    {
      var builder = new StringBuilder();
      builder.AppendLine("|Name|Value|");
      builder.AppendLine("|----|----|");
      builder.AppendLine($"|Fullname|{FullnameNotColored}|");
      builder.AppendLine($"|KindDef|{KindDef}|");
      builder.AppendLine($"|Faction|{Faction}|");
      builder.AppendLine($"|Intelligence|{Intelligence}|");
      builder.AppendLine($"|IsColonist|{IsColonist}|");
      builder.AppendLine($"|IsSlave|{IsSlave}|");
      builder.AppendLine($"|IsPrisoner|{IsPrisoner}|");
      builder.AppendLine($"|HostFaction|{HostFaction}|");
      builder.AppendLine($"|Size|{Size}|");
      builder.AppendLine($"|BioAge|{BioAge}|");
      builder.AppendLine($"|FenceBlocked|{FenceBlocked}|");
      builder.AppendLine($"|IsRoped|{IsRoped}|");
      builder.AppendLine($"|LordJob|{LordJob}|");
      builder.AppendLine($"|IsMutant|{IsMutant}|");
      builder.AppendLine($"|MutantDef|{MutantDef}|");
      builder.AppendLine($"|MutantOpenAnyDoor|{MutantOpenAnyDoor}|");
      return builder;
    }
  }

  public class DebugDialog : Window
  {
    private const float ButtonHeight = 50f;
    private readonly ThingWithComps parent;
    private PawnDebug targetPawn;
    private readonly bool result;
    private readonly StringBuilder builder;
    private readonly Pawn pawn;

    public DebugDialog(ThingWithComps parent, Pawn pawn, StringBuilder builder, bool result)
    {
      this.parent = parent;
      this.pawn = pawn;
      targetPawn = new PawnDebug(pawn);
      this.builder = builder;
      this.result = result;
    }

    public override Vector2 InitialSize => new Vector2(UI.screenWidth * 0.5f, UI.screenHeight * 0.7f);

    public override void DoWindowContents(Rect inRect)
    {
      var leftSide = new Rect(0, 0, inRect.width * 0.45f, inRect.height - ButtonHeight);
      var rightSide = new Rect(inRect.width * 0.5f, 0, inRect.width * 0.45f, inRect.height - ButtonHeight);
      RenderLeftSide(leftSide);
      RenderRightSide(rightSide);
      DoButtons(inRect);
    }

    private void RenderLeftSide(Rect leftSide)
    {
      Listing_Standard listing = new Listing_Standard();
      listing.Begin(leftSide);

      listing.Label($"Player faction: {Faction.OfPlayer}");
      listing.GapLine();

      listing.Label($"Pawn Name: {targetPawn.Fullname}");
      listing.Label($"Pawn KindDef: {targetPawn.KindDef}");
      listing.Label($"Pawn Faction: {targetPawn.Faction}");
      listing.Label($"Intelligence: {targetPawn.Intelligence}");
      listing.Label($"IsColonist: {targetPawn.IsColonist}");
      listing.Label($"IsPrisoner: {targetPawn.IsPrisoner}");
      listing.Label($"IsSlave: {targetPawn.IsSlave}");
      listing.Label($"Pawn Host faction: {targetPawn.HostFaction}");
      listing.Label($"Pawn size: {targetPawn.Size}");
      listing.Label($"Pawn bioAge: {targetPawn.BioAge}");
      listing.Label($"Pawn FenceBlocked: {targetPawn.FenceBlocked}");
      listing.Label($"Pawn is roped: {targetPawn.IsRoped}");
      listing.Label($"Pawn LordJob: {targetPawn.LordJob}");
      listing.Label($"Pawn LordJob can open any door: {targetPawn.LordJobOpenAnyDoor}");
      if (ModsConfig.AnomalyActive)
      {
        listing.Label($"IsMutant: {targetPawn.IsMutant}");
        listing.Label($"Mutant def: {targetPawn.MutantDef}");
        listing.Label($"Mutant can open anyDoor: {targetPawn.MutantOpenAnyDoor}");
      }

      listing.GapLine();

      listing.Label($"Door def: {parent.def.defName}");
      listing.Label($"Door faction: {parent.Faction}");

      listing.GapLine();

      listing.Label($"ChildLock age: {LocksSettings.childLockAge}");
      listing.Label($"Anomalies ignore doors: {LocksSettings.anomaliesIgnoreLocks}");
      listing.Label($"Prisoner break respect doors: {LocksSettings.prisonerBreakRespectsLock}");
      listing.Label($"Revolt respect doors: {LocksSettings.revoltRespectsLocks}");

      listing.End();
    }

    private void RenderRightSide(Rect rightSide)
    {
      Listing_Standard listing = new Listing_Standard();
      listing.Begin(rightSide);
      listing.Label($"Debug logic flow");
      listing.Label(builder.ToString());
      listing.GapLine();
      listing.Label($"Pawn can use doors: {result}");
      listing.End();
    }

    private void DoButtons(Rect inRect)
    {
      var closeRect = new Rect(inRect.width * 0.5f, inRect.height - ButtonHeight, inRect.width * 0.25f, ButtonHeight);
      if (Widgets.ButtonText(closeRect, "Locks_Close".Translate()))
      {
        Close();
      }

      var saveRect = new Rect(inRect.width * 0.25f, inRect.height - ButtonHeight, inRect.width * 0.25f, ButtonHeight);
      if (Widgets.ButtonText(saveRect, "Locks_SaveDebug".Translate()))
      {
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
          $"{parent.def.defName}-vs-{pawn.Name}.md");
        using (var outputFile = new StreamWriter(filePath, true))
        {
          var mdBuilder = new StringBuilder();
          mdBuilder.AppendLine($"# {parent.def.defName}-vs-{pawn.Name}");
          mdBuilder.AppendLine("## Player faction");
          mdBuilder.AppendLine($"{Faction.OfPlayer}");
          mdBuilder.AppendLine("## Pawn");
          mdBuilder.Append(targetPawn.ToMarkdown());
          mdBuilder.AppendLine("## Door");
          mdBuilder.AppendLine("|Name|Value|");
          mdBuilder.AppendLine("|----|----|");
          mdBuilder.AppendLine($"|Door def|{parent.def.defName}|");
          mdBuilder.AppendLine($"|Door faction|{parent.Faction}|");
          mdBuilder.AppendLine("## Door settings");
          mdBuilder.Append(LockUtility.GetRespectedState(parent, pawn).ToMarkdown());
          mdBuilder.AppendLine("## Mod settings");
          mdBuilder.Append(LocksSettings.ToMarkdown());
          mdBuilder.AppendLine($"|Anomaly Active|{ModsConfig.AnomalyActive}|");
          mdBuilder.AppendLine($"## Debug logic flow for result {result}");
          foreach (var line in builder.ToString().Split(Environment.NewLine.ToCharArray()))
          {
            mdBuilder.AppendLine($"- {line}");
          }

          outputFile.WriteLine(mdBuilder.ToString());
          Close();
        }
      }
    }
  }
}