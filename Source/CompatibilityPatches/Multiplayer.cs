using RimWorld;
using Verse;
using Multiplayer.API;

namespace Locks.CompatibilityPatches
{
    class Multiplayer
    {
        [StaticConstructorOnStartup]
        public static class MultiplayerCompatibility
        {
            static MultiplayerCompatibility()
            {
                if (MP.enabled)
                {
                    MP.RegisterAll();
                    MP.RegisterSyncWorker<LockGizmo>(SyncWorkerForLockGizmo);
                    MP.RegisterSyncMethod(typeof(RimWorld.CompAssignableToPawn), nameof(LockData.CompAssignableToPawn.TryAssignPawn)).CancelIfAnyArgNull();
                }
            }
        }
        static void SyncWorkerForLockGizmo(SyncWorker sync, ref LockGizmo inst)
        {
            if (sync.isWriting)
            {
                sync.Write(inst.parent);
            }
            else
            {
                ThingWithComps door = sync.Read<ThingWithComps>();
                inst = new LockGizmo(door);
            }
        }
    }
}
