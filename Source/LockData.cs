using Verse;

namespace Locks
{
  public class LockData : IExposable
  {
    public LockState CurrentState = LockState.DefaultConfiguration();
    public LockState WantedState = LockState.DefaultConfiguration();

    public void ExposeData()
    {
      CurrentState.ExposeData("current");
      WantedState.ExposeData("wanted");
    }

    // Utilities

    #region Utilities

    public bool NeedChange => CurrentState != WantedState;

    public bool CanChangeLocks(Pawn pawn)
    {
      return WantedState.ColonistDoor.Any || WantedState.ColonistDoor.AllowedPawns.Contains(pawn);
    }

    #endregion
  }
}