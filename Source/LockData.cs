using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace Locks
{
    public enum LockMode
    {
        Allies,
        Colony
    }

    public class LockData : IExposable, IAssignableBuilding
    {
        private Building_Door parent;

        public LockMode Mode;
        public bool Locked;
        public bool WantLocked;
        public List<Pawn> Owners;

        public bool Private
        {
            get
            {
                return Owners.Count > 0;
            }
        }

        public IEnumerable<Pawn> AssigningCandidates
        {
            get
            {
                return parent.Map.mapPawns.FreeColonists;
            }
        }

        public IEnumerable<Pawn> AssignedPawns
        {
            get
            {
                return Owners;
            }
        }

        public int MaxAssignedPawnsCount => Owners.Count + 1;

        public LockData()
        {
            Mode = LockMode.Allies;
            Locked = true;
            WantLocked = true;
            Owners = new List<Pawn>();
        }

        public void UpdateReference(Building_Door door)
        {
            parent = door;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref Mode, "Locks_LockData_Mode", LockMode.Allies, false);
            Scribe_Values.Look(ref Locked, "Locks_LockData_Locked", true, false);
            Scribe_Values.Look(ref WantLocked, "Locks_LockData_WantLocked", true, false);
            Scribe_Collections.Look(ref Owners, "Locks_LockData_Owners", LookMode.Reference);
        }

        public void TryAssignPawn(Pawn pawn)
        {
            Owners.Add(pawn);
        }

        public void TryUnassignPawn(Pawn pawn)
        {
            Owners.Remove(pawn);
        }
    }
}
