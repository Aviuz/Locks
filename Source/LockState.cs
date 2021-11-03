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
        public bool allowSlave;
        public bool petDoor;
        public bool pensDoor;
        public bool allowAnimals;
        public List<Pawn> owners;

        public LockState(LockMode mode, bool locked, bool petDoor, bool pensDoor, List<Pawn> owners,
            bool allowSlave = true, bool allowAnimals = true)
        {
            this.mode = mode;
            this.locked = locked;
            this.petDoor = petDoor;
            this.pensDoor = pensDoor;
            this.owners = owners;

            this.allowSlave = allowSlave;
            this.allowAnimals = allowAnimals;
        }

        public void CopyFrom(LockState copy)
        {
            mode = copy.mode;
            locked = copy.locked;
            petDoor = copy.petDoor;
            pensDoor = copy.pensDoor;
            owners.Clear();
            owners.AddRange(copy.owners);

            allowSlave = copy.allowSlave;
            allowAnimals = copy.allowAnimals;
        }

        public static bool operator ==(LockState a, LockState b)
        {
            if (a.mode != b.mode)
            {
                return false;
            }
            if (a.locked != b.locked)
            {
                return false;
            }
            if (a.petDoor != b.petDoor)
            {
                return false;
            }
            if (a.pensDoor != b.pensDoor)
            {
                return false;
            }
            if (a.allowSlave != b.allowSlave)
            {
                return false;
            }
            if (a.allowAnimals != b.allowAnimals)
            {
                return false;
            }
            foreach (var p in a.owners)
            {
                if (!b.owners.Contains(p))
                {
                    return false;
                }
            }
            foreach (var p in b.owners)
            {
                if (!a.owners.Contains(p))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator !=(LockState a, LockState b)
        {
            return !(a == b);
        }

        public bool Private
        {
            get
            {
                return owners.Count > 0;
            }
        }

        public void ExposeData(String postfix)
        {
            Scribe_Values.Look(ref mode, $"Locks_LockData_Mode_{postfix}", LockMode.Allies, false);
            Scribe_Values.Look(ref locked, $"Locks_LockData_Locked_{postfix}", true, false);
            Scribe_Values.Look(ref petDoor, $"Locks_LockData_PetDoor_{postfix}", false, false);
            Scribe_Values.Look(ref pensDoor, $"Locks_LockData_PensDoor_{postfix}", false, false);

            Scribe_Values.Look(ref allowSlave, $"Locks_LockData_SlaveDoor_{postfix}", true, false);
            Scribe_Values.Look(ref allowAnimals, $"Locks_LockData_NonAnimalDoor_{postfix}", true, false);

            Scribe_Collections.Look(ref owners, $"Locks_LockData_Owners_{postfix}", LookMode.Reference);
        }
    }
}
