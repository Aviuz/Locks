using Multiplayer.API;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Locks
{
  public class CopyUtils
  {
    /**
    * Doors in argument insted SelDoor and Data needed for MP
    */
    [SyncMethod(SyncContext.MapSelected)]
    public static void SetWantedStateData(ThingWithComps door, LockState newState)
    {
      var data = LockUtility.GetData(door);

      // Only possible when called from UpdateSettings
      if (newState.owners == null)
        newState.owners = new List<Pawn>(data.WantedState.owners);

      data.WantedState.CopyFrom(newState);
      LockUtility.UpdateLockDesignation(door);

      // Refresh data in multiplayer, as the call to this method will be delayed
      if (MP.IsInMultiplayer && Find.MainTabsRoot.OpenTab == MainButtonDefOf.Inspect && Find.Selector.SingleSelectedObject == door)
      {
        var tab = (MainTabWindow_Inspect)Find.MainTabsRoot.OpenTab.TabWindow;
        tab.CurTabs?.OfType<ITab_Lock>().FirstOrDefault()?.OnOpen();
      }
    }
  }
}