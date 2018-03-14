using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Locks
{
    public enum LockMode
    {
        Allies,
        Colony
    }

    public struct LockState
    {
        public LockMode mode;
        public bool locked;
        public bool petDoor;
        public List<Pawn> owners;

        public LockState(LockMode mode, bool locked, bool petDoor, List<Pawn> owners)
        {
            this.mode = mode;
            this.locked = locked;
            this.petDoor = petDoor;
            this.owners = owners;
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

        public void ExposeData(String postfix)
        {
            Scribe_Values.Look(ref mode, $"Locks_LockData_Mode_{postfix}", LockMode.Allies, false);
            Scribe_Values.Look(ref locked, $"Locks_LockData_Locked_{postfix}", true, false);
            Scribe_Values.Look(ref petDoor, $"Locks_LockData_PetDoor_{postfix}", false, false);
            Scribe_Collections.Look(ref owners, $"Locks_LockData_Owners_{postfix}", LookMode.Reference);
        }
    }
}
