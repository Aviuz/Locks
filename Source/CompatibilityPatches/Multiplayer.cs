using Locks.Commands;
using Multiplayer.API;
using Verse;

namespace Locks.CompatibilityPatches
{
  [StaticConstructorOnStartup]
  public static class MultiplayerCompatibility
  {
    static MultiplayerCompatibility()
    {
      if (!MP.enabled) return;

      MP.RegisterAll();
      MP.RegisterSyncWorker<LockGizmo>(SyncWorkerForLockGizmo);
      MP.RegisterSyncWorker<LockState>(SyncWorkerForLockState);
      MP.RegisterSyncWorker<DoorAllowed>(SyncWorkerForDoorAllowed);
      MP.RegisterSyncWorker<AnimalDoor>(SyncWorkerForAnimalDoor);
      MP.RegisterSyncWorker<MechanoidDoor>(SyncWorkerForMechanoidDoor);
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
    private static void SyncWorkerForDoorAllowed(SyncWorker sync, ref DoorAllowed allowed)
    {
      sync.Bind(ref allowed.Any);
      sync.Bind(ref allowed.AllowedPawns);
    }
    private static void SyncWorkerForAnimalDoor(SyncWorker sync, ref AnimalDoor animal)
    {
      sync.Bind(ref animal.Allowed);
      sync.Bind(ref animal.OnlyPets);
      sync.Bind(ref animal.PensDoor);
    }
    private static void SyncWorkerForMechanoidDoor(SyncWorker sync, ref MechanoidDoor mechanoid)
    {
      sync.Bind(ref mechanoid.Any);
      sync.Bind(ref mechanoid.OnlyMechanitorsMechs);
      sync.Bind(ref mechanoid.AllowedMechanoids);
    }
  }
}
