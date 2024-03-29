using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using System.Reflection;

namespace Locks
{
    public class LockData : IExposable
    {
        private ThingWithComps parent;

        public LockState CurrentState;
        public LockState WantedState;

        public LockData()
        {
            CurrentState = new LockState(LockMode.Allies, true, false, false, new List<Pawn>());
            WantedState = new LockState(LockMode.Allies, true, false, false, new List<Pawn>());
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

        public CompAssignableToPawn CompAssignableToPawn
        {
            get
            {
                var comp = parent.GetComp<CompAssignableToPawn>();
                if (comp == null)
                {
                    comp = new CompAssignableToPawn();
                    comp.parent = parent;
                    var flags = BindingFlags.NonPublic | BindingFlags.Instance;
                    var comps = typeof(ThingWithComps).GetField("comps", flags).GetValue(parent) as List<ThingComp>;
                    comps.Add(comp);
                    comp.Initialize(new CompProperties_AssignableToPawn() { compClass = typeof(CompAssignableToPawn), drawAssignmentOverlay = false, maxAssignedPawnsCount = 999 });
                    typeof(CompAssignableToPawn).GetField("assignedPawns", flags).SetValue(comp, WantedState.owners);
                }
                return comp;
            }
        }

        public void ExposeData()
        {
            CurrentState.ExposeData("current");
            WantedState.ExposeData("wanted");
        }

        public void UpdateReference(ThingWithComps door)
        {
            parent = door;
        }
    }
}
