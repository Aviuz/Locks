using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Locks.Options
{
  class LocksMod : Mod
  {
    private static readonly float GAP_HEIGHT = 12f;
    private const string MOD_NAME = "Locks_ModName";
    private const string CHILD_LOCK = "Locks_ChildrenLock";
    private const string CHILD_LOCK_DESC = "Lock_ChildrenLock_Description";
    private const string PRISON_BREAK = "Locks_PrisonBreak";
    private const string SLAVE_REBELION = "Locks_SlaveRebel";
    private const string ALWAYS_PENS_DOOR = "Locks_AlwaysPensDoor";
    private const string ALWAYS_PENS_DOOR_DESC = "Locks_AlwaysPensDoor_Description";
    private const string ANOMALIES_IGNORE_LOCKS = "Locks_AnomaliesIgnoreLocks";
    private const string ANOMALIES_IGNORE_LOCKS_DESC = "Locks_AnomaliesIgnoreLocks_Description";
    private const string DEBUG_BUTTON = "Locks_DebugWidget";
    private const string DEBUG_BUTTON_DESC = "Locks_DebugWidget_Description";

    public LocksMod(ModContentPack content) : base(content)
    {
      this.GetSettings<LocksSettings>();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
      Listing_Standard listingStandard = new Listing_Standard();

      listingStandard.Begin(inRect);
      listingStandard.Gap(GAP_HEIGHT);

      listingStandard.Label(CHILD_LOCK.Translate(LocksSettings.childLockAge), tooltip: CHILD_LOCK_DESC.Translate());
      LocksSettings.childLockAge = (int)listingStandard.Slider(LocksSettings.childLockAge, 0, 18);
      listingStandard.Gap(GAP_HEIGHT);

      listingStandard.CheckboxLabeled(PRISON_BREAK.Translate(), ref LocksSettings.prisonerBreakRespectsLock);
      listingStandard.Gap(GAP_HEIGHT);

      listingStandard.CheckboxLabeled(SLAVE_REBELION.Translate(), ref LocksSettings.revoltRespectsLocks);
      listingStandard.Gap(GAP_HEIGHT);

      listingStandard.CheckboxLabeled(ALWAYS_PENS_DOOR.Translate(), ref LocksSettings.alwaysPensDoor,
        ALWAYS_PENS_DOOR_DESC.Translate());
      listingStandard.Gap(GAP_HEIGHT);

      if (ModsConfig.AnomalyActive)
      {
        listingStandard.CheckboxLabeled(ANOMALIES_IGNORE_LOCKS.Translate(), ref LocksSettings.anomaliesIgnoreLocks,
          ANOMALIES_IGNORE_LOCKS_DESC.Translate());
        listingStandard.Gap(GAP_HEIGHT);
      }

      listingStandard.CheckboxLabeled(DEBUG_BUTTON.Translate(), ref LocksSettings.debugButton,
        DEBUG_BUTTON_DESC.Translate());
      listingStandard.Gap(GAP_HEIGHT);

      listingStandard.End();
    }

    public override string SettingsCategory()
    {
      return MOD_NAME.Translate();
    }
  }
}