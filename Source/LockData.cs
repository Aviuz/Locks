using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace Locks
{
    public class LockData : CompAssignableToPawn, IExposable
    {
        private Building_Door parent;

        public LockState CurrentState;
        public LockState WantedState;

        public LockData()
        {
            CurrentState = new LockState(LockMode.Allies, true, false, new List<Pawn>());
            WantedState = new LockState(LockMode.Allies, true, false, new List<Pawn>());
        }

        // Utilities
        #region Utilities
        public bool NeedChange
        {
            get
            {
                return CurrentState != WantedState;
            }
        }

        public bool CanChangeLocks(Pawn pawn)
        {
            return WantedState.owners.Count == 0 || WantedState.owners.Contains(pawn);
        }
        #endregion

        public override void TryAssignPawn(Pawn pawn)
        {
            base.TryAssignPawn(pawn);
            UpdateOwners();
        }

        public override void TryUnassignPawn(Pawn pawn, bool sort = true)
        {
            base.TryUnassignPawn(pawn, sort);
            UpdateOwners();
        }

        public void UpdateOwners()
        {
            foreach (Building_Door door in Find.Selector.SelectedObjects.Where(o => o is Building_Door && o != parent))
            {
                LockUtility.GetData(door).WantedState.owners.Clear();
                LockUtility.GetData(door).WantedState.owners.AddRange(LockUtility.GetData(parent).WantedState.owners);
                LockUtility.UpdateLockDesignation(door);
            }
            LockUtility.UpdateLockDesignation(parent);
        }

        public void ExposeData()
        {
            CurrentState.ExposeData("current");
            WantedState.ExposeData("wanted");
        }

        public void UpdateReference(Building_Door door)
        {
            parent = door;
        }
    }
}
