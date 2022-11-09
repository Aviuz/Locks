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
                    MP.RegisterSyncWorker<LockState>(SyncWorkerForLockState);
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

        static void SyncWorkerForLockState(SyncWorker sync, ref LockState state)
        {
            sync.Bind(ref state.mode);
            sync.Bind(ref state.locked);
            sync.Bind(ref state.allowSlave);
            sync.Bind(ref state.petDoor);
            sync.Bind(ref state.pensDoor);
            sync.Bind(ref state.allowAnimals);
            sync.Bind(ref state.childLock);
            sync.Bind(ref state.owners);
        }
    }
}
