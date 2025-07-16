using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Locks.Debug;
using Verse;

namespace Locks
{
  public enum LockMode
  {
    Allies,
    Colony
  }

  public struct DoorAllowed : IEquatable<DoorAllowed>, IMarkdown
  {
    public bool Any;
    public List<Pawn> AllowedPawns;

    public DoorAllowed(bool any, List<Pawn> allowedPawns = null)
    {
      Any = any;
      AllowedPawns = allowedPawns;
    }

    public DoorAllowed(DoorAllowed copy) : this(copy.Any, new List<Pawn>(copy.AllowedPawns))
    {
    }

    public bool IsAllowed(Pawn pawn)
    {
      return Any || (AllowedPawns?.Contains(pawn) ?? false);
    }

    public bool Equals(DoorAllowed other)
    {
      return Any == other.Any &&
             AllowedPawns.Count == other.AllowedPawns.Count
             && AllowedPawns.All(other.AllowedPawns.Contains);
    }

    public override bool Equals(object obj)
    {
      return obj is DoorAllowed other && Equals(other);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (Any.GetHashCode() * 397) ^ (AllowedPawns != null ? AllowedPawns.GetHashCode() : 0);
      }
    }

    public StringBuilder ToMarkdown()
    {
      var builder = new StringBuilder();
      builder.AppendLine("|Name|Value|");
      builder.AppendLine("|----|----|");
      builder.AppendLine($"|Any|{Any}|");
      builder.AppendLine();
      builder.AppendLine("Allowed pawns:");
      AllowedPawns?.ForEach(pawn => builder.AppendLine($"- {pawn.Name}({pawn})"));
      return builder;
    }
  }

  public struct MechanoidDoor : IEquatable<MechanoidDoor>, IMarkdown
  {
    public bool Any;
    public bool OnlyMechanitorsMechs;
    public List<string> AllowedMechanoids;

    public MechanoidDoor(bool any, bool onlyMechanitorsMechs, List<string> allowedMechanoids)
    {
      Any = any;
      OnlyMechanitorsMechs = onlyMechanitorsMechs;
      AllowedMechanoids = allowedMechanoids;
    }

    public MechanoidDoor(MechanoidDoor copy) : this(copy.Any, copy.OnlyMechanitorsMechs,
      new List<string>(copy.AllowedMechanoids))
    {
    }

    public bool Equals(MechanoidDoor other)
    {
      return Any == other.Any && OnlyMechanitorsMechs == other.OnlyMechanitorsMechs &&
             AllowedMechanoids.Count == other.AllowedMechanoids.Count
             && AllowedMechanoids.All(other.AllowedMechanoids.Contains);
    }

    public override bool Equals(object obj)
    {
      return obj is MechanoidDoor other && Equals(other);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = Any.GetHashCode();
        hashCode = (hashCode * 397) ^ OnlyMechanitorsMechs.GetHashCode();
        hashCode = (hashCode * 397) ^ (AllowedMechanoids != null ? AllowedMechanoids.GetHashCode() : 0);
        return hashCode;
      }
    }

    public StringBuilder ToMarkdown()
    {
      var builder = new StringBuilder();
      builder.AppendLine("|Name|Value|");
      builder.AppendLine("|----|----|");
      builder.AppendLine($"|Any|{Any}|");
      builder.AppendLine($"|OnlyMechanitorsMechs|{OnlyMechanitorsMechs}|");
      builder.AppendLine();
      builder.AppendLine("AllowedMechanoids:");
      AllowedMechanoids?.ForEach(def => builder.AppendLine($"- {def}"));
      return builder;
    }
  }

  public struct AnimalDoor : IEquatable<AnimalDoor>, IMarkdown
  {
    public bool Allowed;
    public bool OnlyPets;
    public bool PensDoor;

    public AnimalDoor(bool allowed, bool onlyPets, bool pensDoor)
    {
      Allowed = allowed;
      OnlyPets = onlyPets;
      PensDoor = pensDoor;
    }

    public AnimalDoor(AnimalDoor copy) : this(copy.Allowed, copy.OnlyPets, copy.PensDoor)
    {
    }

    public bool Equals(AnimalDoor other)
    {
      return Allowed == other.Allowed && OnlyPets == other.OnlyPets && PensDoor == other.PensDoor;
    }

    public override bool Equals(object obj)
    {
      return obj is AnimalDoor other && Equals(other);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = Allowed.GetHashCode();
        hashCode = (hashCode * 397) ^ OnlyPets.GetHashCode();
        hashCode = (hashCode * 397) ^ PensDoor.GetHashCode();
        return hashCode;
      }
    }

    public StringBuilder ToMarkdown()
    {
      var builder = new StringBuilder();
      builder.AppendLine("|Name|Value|");
      builder.AppendLine("|----|----|");
      builder.AppendLine($"|Allowed|{Allowed}|");
      builder.AppendLine($"|OnlyPets|{OnlyPets}|");
      builder.AppendLine($"|PensDoor|{PensDoor}|");
      return builder;
    }
  }

  public struct LockState : IEquatable<LockState>, IMarkdown
  {
    public LockMode Mode;
    public bool Locked;
    public bool ChildLock;
    public AnimalDoor AnimalDoor;
    public MechanoidDoor MechanoidDoor;
    public DoorAllowed ColonistDoor;
    public DoorAllowed SlaveAllowed;

    public static LockState DefaultConfiguration()
    {
      return new LockState(LockMode.Allies, true, true, new AnimalDoor(true, false, false),
        new DoorAllowed(true, new List<Pawn>()), new DoorAllowed(true, new List<Pawn>()),
        new MechanoidDoor(true, false, new List<string>()));
    }

    public LockState(LockMode mode, bool locked, bool childLock, AnimalDoor animalDoor, DoorAllowed colonistDoor,
      DoorAllowed slaveAllowed, MechanoidDoor mechanoidDoor)
    {
      Mode = mode;
      Locked = locked;
      ChildLock = childLock;
      AnimalDoor = animalDoor;
      ColonistDoor = colonistDoor;
      SlaveAllowed = slaveAllowed;
      MechanoidDoor = mechanoidDoor;
    }

    public bool Equals(LockState other)
    {
      return Mode == other.Mode && Locked == other.Locked && ChildLock == other.ChildLock &&
             AnimalDoor.Equals(other.AnimalDoor) && MechanoidDoor.Equals(other.MechanoidDoor) &&
             ColonistDoor.Equals(other.ColonistDoor) && SlaveAllowed.Equals(other.SlaveAllowed);
    }

    public override bool Equals(object obj)
    {
      return obj is LockState other && Equals(other);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = (int)Mode;
        hashCode = (hashCode * 397) ^ Locked.GetHashCode();
        hashCode = (hashCode * 397) ^ ChildLock.GetHashCode();
        hashCode = (hashCode * 397) ^ AnimalDoor.GetHashCode();
        hashCode = (hashCode * 397) ^ MechanoidDoor.GetHashCode();
        hashCode = (hashCode * 397) ^ ColonistDoor.GetHashCode();
        hashCode = (hashCode * 397) ^ SlaveAllowed.GetHashCode();
        return hashCode;
      }
    }

    public StringBuilder ToMarkdown()
    {
      var builder = new StringBuilder();
      builder.AppendLine("|Name|Value|");
      builder.AppendLine("|----|----|");
      builder.AppendLine($"|Mode|{Mode}|");
      builder.AppendLine($"|Locked|{Locked}|");
      builder.AppendLine($"|ChildLock|{ChildLock}|");
      builder.AppendLine("### AnimalDoor");
      builder.Append(AnimalDoor.ToMarkdown());
      builder.AppendLine("### ColonistDoor");
      builder.Append(ColonistDoor.ToMarkdown());
      builder.AppendLine("### SlaveAllowed");
      builder.Append(SlaveAllowed.ToMarkdown());
      builder.AppendLine("### MechanoidDoor");
      builder.Append(MechanoidDoor.ToMarkdown());
      return builder;
    }

    public void CopyFrom(LockState copy)
    {
      Mode = copy.Mode;
      Locked = copy.Locked;
      ChildLock = copy.ChildLock;

      ColonistDoor = new DoorAllowed(copy.ColonistDoor);
      SlaveAllowed = new DoorAllowed(copy.SlaveAllowed);
      AnimalDoor = new AnimalDoor(copy.AnimalDoor);
      MechanoidDoor = new MechanoidDoor(copy.MechanoidDoor);
    }

    public static bool operator ==(LockState a, LockState b)
    {
      return Equals(a, b);
    }

    public static bool operator !=(LockState a, LockState b)
    {
      return !Equals(a, b);
    }

    public bool Private => !ColonistDoor.Any;

    public void ExposeData(string postfix)
    {
      Scribe_Values.Look(ref Mode, $"Locks_LockData_Mode_{postfix}");
      Scribe_Values.Look(ref Locked, $"Locks_LockData_Locked_{postfix}", true);
      Scribe_Values.Look(ref ChildLock, $"Locks_LockData_ChildLock_{postfix}");

      Scribe_Values.Look(ref AnimalDoor.Allowed, $"Locks_LockData_NonAnimalDoor_{postfix}", true);
      Scribe_Values.Look(ref AnimalDoor.OnlyPets, $"Locks_LockData_PetDoor_{postfix}");
      Scribe_Values.Look(ref AnimalDoor.PensDoor, $"Locks_LockData_PensDoor_{postfix}");

      Scribe_Values.Look(ref SlaveAllowed.Any, $"Locks_LockData_SlaveDoor_{postfix}", true);
      Scribe_Collections.Look(ref SlaveAllowed.AllowedPawns, $"Locks_LockData_AllowedSlaves_{postfix}",
        LookMode.Reference);

      Scribe_Values.Look(ref ColonistDoor.Any, $"Locks_LockData_AnyColonist_{postfix}", true);
      Scribe_Collections.Look(ref ColonistDoor.AllowedPawns, $"Locks_LockData_Owners_{postfix}", LookMode.Reference);

      Scribe_Values.Look(ref MechanoidDoor.Any, $"Locks_LockData_AnyMech_{postfix}", true);
      Scribe_Values.Look(ref MechanoidDoor.OnlyMechanitorsMechs, $"Locks_LockData_OnlyMechanitorsMechs_{postfix}",
        true);
      Scribe_Collections.Look(ref MechanoidDoor.AllowedMechanoids, $"Locks_LockData_AllowedMechs_{postfix}",
        LookMode.Value);

      if (Scribe.mode == LoadSaveMode.PostLoadInit)
      {
        ColonistDoor.AllowedPawns?.RemoveAll(x => x == null);
        SlaveAllowed.AllowedPawns?.RemoveAll(x => x == null);
        MechanoidDoor.AllowedMechanoids?.RemoveAll(x => x == null);
      }
    }
  }
}