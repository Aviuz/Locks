using System.Linq;
using Multiplayer.API;
using RimWorld;
using Verse;

namespace Locks
{
  public class CopyUtils
  {
    /**
    * Doors in argument instead SelDoor and Data needed for MP
    */
    [SyncMethod(SyncContext.MapSelected)]
    public static void SetWantedStateData(ThingWithComps door, LockState newState)
    {
      var data = LockUtility.GetData(door);

      data.WantedState.CopyFrom(newState);
      LockUtility.UpdateLockDesignation(door);

      // Refresh data in multiplayer, as the call to this method will be delayed
      if (MP.IsInMultiplayer && Find.MainTabsRoot.OpenTab == MainButtonDefOf.Inspect &&
          Find.Selector.SingleSelectedObject == door)
      {
        var tab = (MainTabWindow_Inspect)Find.MainTabsRoot.OpenTab.TabWindow;
        tab.CurTabs?.OfType<ITab_Lock>().FirstOrDefault()?.OnOpen();
      }
    }
  }
}