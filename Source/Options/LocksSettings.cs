using System.Text;
using Verse;

namespace Locks.Options
{
  public class LocksSettings : ModSettings
  {
    public static int childLockAge = 6;
    public static bool prisonerBreakRespectsLock = true;
    public static bool revoltRespectsLocks = true;
    public static bool alwaysPensDoor;
    public static bool anomaliesIgnoreLocks = true;
    public static bool debugButton = true;

    public override void ExposeData()
    {
      Scribe_Values.Look(ref childLockAge, "Locks_childLockAge", 6, true);

      Scribe_Values.Look(ref prisonerBreakRespectsLock, "Locks_BreakRespectsLocks", true, true);
      Scribe_Values.Look(ref revoltRespectsLocks, "Locks_RevoltRespectLocks", true, true);

      Scribe_Values.Look(ref alwaysPensDoor, "Locks_AlwaysPensDoor", false, true);
      Scribe_Values.Look(ref anomaliesIgnoreLocks, "Locks_AnomaliesIgnoreLocks", true, true);
      Scribe_Values.Look(ref debugButton, "Locks_DebugButton", true, true);
    }

    public static StringBuilder ToMarkdown()
    {
      var builder = new StringBuilder();
      builder.AppendLine("|Name|Value|");
      builder.AppendLine("|----|----|");
      builder.AppendLine($"|childLockAge|{childLockAge}|");
      builder.AppendLine($"|prisonerBreakRespectsLock|{prisonerBreakRespectsLock}|");
      builder.AppendLine($"|revoltRespectsLocks|{revoltRespectsLocks}|");
      builder.AppendLine($"|alwaysPensDoor|{alwaysPensDoor}|");
      builder.AppendLine($"|anomaliesIgnoreLocks|{anomaliesIgnoreLocks}|");
      builder.AppendLine($"|debugButton|{debugButton}|");
      return builder;
    }
  }
}