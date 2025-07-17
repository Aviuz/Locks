using Multiplayer.API;
using Verse;

namespace Locks.CompatibilityPatches
{
  [StaticConstructorOnStartup]
  public static class MultiplayerCompatibility
  {
    static MultiplayerCompatibility()
    {
      if (!MP.enabled)
      {
        return;
      }

      MP.RegisterAll();
      MP.RegisterSyncWorker<LockGizmo>(SyncWorkerForLockGizmo);
      MP.RegisterSyncWorker<LockState>(SyncWorkerForLockState);
    }

    private static void SyncWorkerForLockGizmo(SyncWorker sync, ref LockGizmo inst)
    {
      if (sync.isWriting)
      {
        sync.Write(inst.parent);
      }
      else
      {
        var door = sync.Read<ThingWithComps>();
        inst = new LockGizmo(door);
      }
    }

    private static void SyncWorkerForLockState(SyncWorker sync, ref LockState state)
    {
      sync.Bind(ref state.Mode);
      sync.Bind(ref state.Locked);
      sync.Bind(ref state.ChildLock);

      sync.Bind(ref state.ColonistDoor);
      sync.Bind(ref state.SlaveAllowed);
      sync.Bind(ref state.AnimalDoor);
      sync.Bind(ref state.MechanoidDoor);
    }
  }
}