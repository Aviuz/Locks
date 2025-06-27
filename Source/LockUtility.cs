using Locks.CompatibilityPatches;
using Locks.Options;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Multiplayer.API;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace Locks
{
  public static class LockUtility
  {
    private static readonly float MaxPetSize = 0.86f;

    private static readonly Dictionary<ThingWithComps, LockData> Map = new Dictionary<ThingWithComps, LockData>();

    public static List<PawnKindDef> MechKinds { get; } = DefDatabase<PawnKindDef>.AllDefs
      .Where(def => def.defName.StartsWith("Mech_")).OrderBy(def => def.defName)
      .ToList();


    private static DesignationDef designationDef;
    private static JobDef jobDef;

    public static DesignationDef DesDef =>
      designationDef ?? (designationDef = DefDatabase<DesignationDef>.GetNamed("Locks_Flick"));

    public static JobDef JobDef => jobDef ?? (jobDef = DefDatabase<JobDef>.GetNamed("Locks_Flick"));

    public static bool PawnCanOpen(ThingWithComps door, Pawn p)
    {
      return PawnCanOpenLogged(door, p);
    }

    public static bool PawnCanOpenLogged(ThingWithComps door, Pawn p, StringBuilder builder = null)
    {
      bool canOpenAnyDoor = p.GetLord()?.LordJob?.CanOpenAnyDoor(p) ?? false;
      bool noFaction = door.Faction == null;
      bool specialGuest = p.guest?.Released ?? false;

      if (canOpenAnyDoor || specialGuest)
      {
        builder?.AppendLine(
          $"Special rule. LordJob open any door: {canOpenAnyDoor}, special guest: {specialGuest}. LordJob: {p.GetLord()?.LordJob?.GetType().Name}");

        return true;
      }

      if (ModsConfig.AnomalyActive && p.IsMutant && !p.mutant.Def.canOpenDoors)
      {
        builder?.AppendLine("Anomaly mutant should not open doors");
        return false;
      }

      if (noFaction)
      {
        builder?.AppendLine($"Doors without faction. Returning pawn RaceProps: {p.RaceProps.canOpenFactionlessDoors}");
        return p.RaceProps.canOpenFactionlessDoors;
      }

      var respectedState = GetRespectedState(door, p);

      if (!(p.RaceProps is RaceProperties properties))
      {
        builder?.AppendLine("Pawn doesn't have RaceProps set");
        return HandleAnomalies(p, builder);
      }

      if (properties.Humanlike)
      {
        builder?.AppendLine("Pawn recognized as Humanlike");
        return !respectedState.Locked || HandleHumanoids(door, p, respectedState, builder);
      }

      if (properties.Animal)
      {
        builder?.AppendLine("Pawn recognized as Animal");
        return HandleAnimals(door, p, respectedState, builder);
      }

      if (properties.IsMechanoid)
      {
        builder?.AppendLine("Pawn recognized as Mechanoid");
        return respectedState.Locked && HandleMechanoid(door, p, respectedState, builder);
      }

      builder?.AppendLine("Pawn doesn't match any type.");
      return HandleAnomalies(p, builder);
    }

    public static LockState GetRespectedState(ThingWithComps door, Pawn p)
    {
      LockState respectedState;
      if (p.IsPrisoner || door.Faction.HostileTo(p.Faction) || p.InMentalState)
      {
        respectedState = GetData(door).CurrentState;
      }
      else
      {
        respectedState = GetData(door).WantedState;
      }

      return respectedState;
    }

    private static bool HandleAnomalies(Pawn p, StringBuilder builder)
    {
      if (ModsConfig.AnomalyActive)
      {
        builder?.AppendLine(
          $"Anomalies check. IgnoreLock: {LocksSettings.anomaliesIgnoreLocks}, pawn is revenant: {p.kindDef == PawnKindDefOf.Revenant}, mutant can open any door: {p.IsMutant && p.mutant.Def.canOpenAnyDoor}");
        return LocksSettings.anomaliesIgnoreLocks &&
               (p.IsMutant && p.mutant.Def.canOpenAnyDoor || p.kindDef == PawnKindDefOf.Revenant);
      }

      builder?.AppendLine("Anomaly is not active");
      return false;
    }

    private static bool HandleMechanoid(ThingWithComps door, Pawn pawn, LockState respectedState, StringBuilder builder)
    {
      if (pawn.Faction.HostileTo(door.Faction))
      {
        builder?.AppendLine("Pawn is hostile");
        return false;
      }

      if (respectedState.MechanoidDoor.OnlyMechanitorsMechs)
      {
        builder?.AppendLine($"Looking for any mechanitor in allowed colonists.");
        return respectedState.ColonistDoor.Any || OwnersControlMech(respectedState, pawn);
      }

      builder?.AppendLine("Checking for any mech or in allowed mechs pool.");
      return respectedState.MechanoidDoor.Any ||
             respectedState.MechanoidDoor.AllowedMechanoids.Contains(pawn.kindDef.defName);
    }

    private static bool HandleAnimals(ThingWithComps door, Pawn pawn, LockState respectedState, StringBuilder builder)
    {
      if (pawn.Faction.HostileTo(door.Faction) || !respectedState.AnimalDoor.Allowed)
      {
        builder?.AppendLine("Pawn is hostile or animals not allowed");
        return false;
      }

      builder?.AppendLine("Checking pet size and faction.");
      if (respectedState.AnimalDoor.OnlyPets && pawn.RaceProps.baseBodySize <= MaxPetSize &&
          pawn.Faction == door.Faction)
      {
        builder?.AppendLine("Pet in allowed size");
        return true;
      }

      builder?.AppendLine("Checking for pens configuration");
      if (respectedState.AnimalDoor.PensDoor && pawn.RaceProps.FenceBlocked && !door.def.building.roamerCanOpen &&
          (!pawn.roping.IsRopedByPawn || !PawnCanOpen(door, pawn.roping.RopedByPawn)))
      {
        builder?.AppendLine("Animal should not enter pen. Checking LordJob configuration");
        return pawn.GetLord()?.LordJob is LordJob_TradeWithColony;
      }

      if (pawn.Faction != door.Faction && pawn.Faction != null && respectedState.Mode == LockMode.Colony)
      {
        builder?.AppendLine("No allies allowed");
        return false;
      }

      if (pawn.Faction == null)
      {
        builder?.AppendLine("Animal without faction. Return false");
        return false;
      }

      builder?.AppendLine("No rules matching here. Return true.");
      return true;
    }

    private static bool HandleHumanoids(ThingWithComps door, Pawn pawn, LockState respectedState, StringBuilder builder)
    {
      if (pawn.Faction.HostileTo(door.Faction))
      {
        builder?.AppendLine("Pawn is hostile");
        return false;
      }

      if (pawn.Faction == door.Faction)
      {
        builder?.AppendLine("Pawn and door share faction");
        if (respectedState.ChildLock && pawn.ageTracker != null &&
            pawn.ageTracker.AgeBiologicalYears < LocksSettings.childLockAge)
        {
          builder?.AppendLine("Child lock rule triggered");
          return false;
        }

        builder?.AppendLine("Checking rules for free pawns");
        if (pawn.IsFreeNonSlaveColonist)
        {
          builder?.AppendLine("Checking if colonist can use door");
          return respectedState.ColonistDoor.IsAllowed(pawn);
        }

        builder?.AppendLine("Checking rules for slaves");
        if (pawn.IsSlave)
        {
          builder?.AppendLine("Checking is slave can use door");
          return respectedState.SlaveAllowed.IsAllowed(pawn);
        }

        if ((door.Map?.Parent.doorsAlwaysOpenForPlayerPawns ?? false) && !pawn.IsPrisonerOfColony)
        {
          builder?.AppendLine("Free colonist with doorsAlwaysOpenForPlayerPawns");
          return true;
        }
      }

      if (respectedState.Mode == LockMode.Allies)
      {
        builder?.AppendLine("Checking allies mode");
        return (pawn.guest?.Released ?? false) || pawn.IsFreeman || pawn.HostFaction != door.Faction ||
               WildManUtility.WildManShouldReachOutsideNow(pawn);
      }

      builder?.AppendLine("No other options for Humanoids");
      return false;
    }

    private static bool OwnersControlMech(LockState respectedState, Pawn p)
    {
      return p.IsColonyMech && Enumerable.Any(respectedState.ColonistDoor.AllowedPawns,
        owner => owner?.mechanitor != null && owner.mechanitor.ControlledPawns.Contains(p));
    }

    public static LockData GetData(ThingWithComps key)
    {
      if (!Map.ContainsKey(key))
        Map[key] = new LockData();
      return Map[key];
    }

    public static void Remove(ThingWithComps key)
    {
      Map.Remove(key);
    }

    public static void UpdateLockDesignation(Thing t)
    {
      bool flag = false;
      ThingWithComps door = t as ThingWithComps;
      if (door.TryGetComp<CompLock>() != null)
      {
        flag = GetData(door).NeedChange;
      }

      Designation designation = t.Map.designationManager.DesignationOn(t, DesDef);
      if (flag && designation == null)
      {
        t.Map.designationManager.AddDesignation(new Designation(t, DesDef));
        door.Map.reachability.ClearCache();
      }
      else if (!flag && designation != null)
      {
        designation.Delete();
      }
    }

    public static bool IsVisible(this LockState state, string propertyName)
    {
      switch (propertyName)
      {
        case nameof(LockState.Locked):
          return true;
        case nameof(LockState.Mode):
          return state.Locked && !state.Private;
        case nameof(LockState.AnimalDoor):
          return state.Locked;
        case nameof(LockState.AnimalDoor.PensDoor):
          return true;
        case nameof(LockState.ColonistDoor.AllowedPawns):
          return state.Locked;
        case nameof(LockState.AnimalDoor.Allowed):
          return state.Locked;
        case nameof(LockState.SlaveAllowed.Any):
          return state.Locked;
        case nameof(LockState.ChildLock):
          return state.Locked;
        default:
          return true;
      }
    }
  }
}