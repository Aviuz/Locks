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
            this.compClass = typeof(CompLock);
        }
    }

    public class CompLock : ThingComp
    {
        private static readonly string FENCE = "FenceGate";
        public override string CompInspectStringExtra()
        {
            string text = "Locks_StatePrefix".Translate() + " ";

            if (LockUtility.GetData(this.parent).CurrentState.locked)
                text += "Locks_StateLocked".Translate();
            else
                text += "Locks_StateUnlocked".Translate();
            if (LockUtility.GetData(this.parent).NeedChange)
                text += $" ({"Locks_StateChanging".Translate()})";

            return text;
        }

        public override void PostDeSpawn(Map map)
        {
            LockUtility.Remove(this.parent);
        }

        public override void PostExposeData()
        {
            LockUtility.GetData(this.parent).ExposeData();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return new LockGizmo(this.parent);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (this.parent.def.defName == FENCE)
            {
                LockUtility.GetData(this.parent).CurrentState.pensDoor = true;
                LockUtility.GetData(this.parent).WantedState.pensDoor = true;
            }
        }
    }
}
