using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Locks
{
    public class CompProperties_Lock : CompProperties
    {
        public CompProperties_Lock()
        {
            compClass = typeof(CompLock);
        }
    }

    public class CompLock : ThingComp, IAssignableBuilding
    {
        public LockState currentState = new LockState();
        public LockState wantedState = new LockState();
        
        public IEnumerable<Pawn> AssigningCandidates => parent.Map.mapPawns.FreeColonists;
        public IEnumerable<Pawn> AssignedPawns => wantedState.AssignedPawns;
        public int MaxAssignedPawnsCount => wantedState.owners.Count + 1;
        
        public bool NeedChange => currentState != wantedState;
        public bool Locked => currentState.locked;
        
        public bool CanChangeLocks(Pawn pawn)
        {
            return wantedState.owners.Count == 0 || wantedState.owners.Contains(pawn);
        }
        
        public override string CompInspectStringExtra()
        {
            string text = "Locks_StatePrefix".Translate() + " ";

            if (currentState.locked)
                text += "Locks_StateLocked".Translate();
            else
                text += "Locks_StateUnlocked".Translate();
            if (NeedChange)
                text += $" ({"Locks_StateChanging".Translate()})";

            return text;
        }

        public void Update()
        {
            currentState.CopyFrom(wantedState);
        }

        public void TryAssignPawn(Pawn pawn)
        {
            wantedState.TryAssignPawn(pawn);
        }

        public void TryUnassignPawn(Pawn pawn)
        {
            wantedState.TryUnassignPawn(pawn);
        }

        public bool AssignedAnything(Pawn pawn)
        {
            return false;
        }

        public override void PostExposeData()
        {
            Scribe_Deep.Look(ref currentState, "current", new LockState());
            Scribe_Deep.Look(ref wantedState, "wanted", new LockState());
        }
    }
}
