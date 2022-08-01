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
        private const string MOD_NAME = "Locks_ModName";
        private const string CHILD_LOCK = "Locks_ChildrenLock";
        private const string CHILD_LOCK_DESC = "Lock_ChildrenLock_Description";
        private const string PRISON_BREAK= "Locks_PrisonBreak";
        private const string SLAVE_REBELION ="Locks_SlaveRebel";
        private const string ALWAYS_PENS_DOOR = "Locks_AlwaysPensDoor";
        private const string ALWAYS_PENS_DOOR_DESC = "Locks_AlwaysPensDoor_Description";

        public LocksMod(ModContentPack content) : base(content)
        {
            this.GetSettings<LocksSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();

            listingStandard.Begin(inRect);
            listingStandard.Gap(12f);

            listingStandard.Label(CHILD_LOCK.Translate(LocksSettings.childLockAge), tooltip: CHILD_LOCK_DESC.Translate());
            LocksSettings.childLockAge = (int)listingStandard.Slider(LocksSettings.childLockAge, 0, 18);
            listingStandard.Gap(12f);

            listingStandard.CheckboxLabeled(PRISON_BREAK.Translate(), ref LocksSettings.prisonerBreakRespectsLock);
            listingStandard.Gap(12f);

            listingStandard.CheckboxLabeled(SLAVE_REBELION.Translate(), ref LocksSettings.revoltRespectsLocks);
            listingStandard.Gap(12f);

            listingStandard.CheckboxLabeled(ALWAYS_PENS_DOOR.Translate(), ref LocksSettings.alwaysPensDoor, ALWAYS_PENS_DOOR_DESC.Translate());
            listingStandard.Gap(12f);

            listingStandard.End();
        }

        public override string SettingsCategory()
        {
            return MOD_NAME.Translate();
        }
    }
}
