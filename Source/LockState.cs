using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using RimWorld;
using Verse;

namespace Locks
{
    public enum LockMode
    {
        Allies,
        Colony
    }

    public class LockState : IExposable, IAssignableBuilding
    {
        public LockMode mode;
        public bool locked;
        public bool petDoor;
        public List<Pawn> owners;

        public IEnumerable<Pawn> AssigningCandidates => owners.AsEnumerable();
        public IEnumerable<Pawn> AssignedPawns => owners.AsEnumerable();
        public int MaxAssignedPawnsCount => owners.Count + 1;

        public bool Private => owners.Count > 0;

        public LockState()
        {
            mode = LockMode.Allies;
            locked = true;
            petDoor = false;
            owners = new List<Pawn>();
        }
        
        public LockState(LockMode mode, bool locked, bool petDoor, List<Pawn> owners)
        {
            this.mode = mode;
            this.locked = locked;
            this.petDoor = petDoor;
            this.owners = owners;
        }
        
        public LockState(LockState other)
        {
            mode = other.mode;
            locked = other.locked;
            petDoor = other.petDoor;
            owners = new List<Pawn>();
            owners.AddRange(other.owners);
        }

        public void CopyFrom(LockState copy)
        {
            mode = copy.mode;
            locked = copy.locked;
            petDoor = copy.petDoor;
            owners.Clear();
            owners.AddRange(copy.owners);
        }

        public static bool operator ==(LockState a, LockState b)
        {
            if (a.mode != b.mode)
                return false;
            if (a.locked != b.locked)
                return false;
            if (a.petDoor != b.petDoor)
                return false;
            foreach (var p in a.owners)
                if (!b.owners.Contains(p))
                    return false;
            foreach (var p in b.owners)
                if (!a.owners.Contains(p))
                    return false;
            return true;
        }

        public static bool operator !=(LockState a, LockState b)
        {
            if (a.mode != b.mode)
                return true;
            if (a.locked != b.locked)
                return true;
            if (a.petDoor != b.petDoor)
                return true;
            foreach (var p in a.owners)
                if (!b.owners.Contains(p))
                    return true;
            foreach (var p in b.owners)
                if (!a.owners.Contains(p))
                    return true;
            return false;
        }

        public void TryAssignPawn(Pawn pawn)
        {
            if(!owners.Contains(pawn))
                owners.Add(pawn);
        }

        public void TryUnassignPawn(Pawn pawn)
        {
            owners.Remove(pawn);
        }

        public bool AssignedAnything(Pawn pawn)
        {
            return false;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref mode, "mode", LockMode.Allies, false);
            Scribe_Values.Look(ref locked, "locked", true, false);
            Scribe_Values.Look(ref petDoor, "petDoor", false, false);
            Scribe_Collections.Look(ref owners, "owners", LookMode.Reference);
        }
    }
}
