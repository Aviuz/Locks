using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Locks
{
    public class ITab_Lock : ITab
    {
        private Vector2 scrollPosition;

        private static readonly Vector2 WinSize = new Vector2(310f + WindowGap, ButtonsHeight + OwnersWindowHeight + OwnersTitleHeight + MainSettingsHeight + WindowGap + 2 * Spacing + WarningHeight);
        private const float ButtonsHeight = 25f;
        private const float WindowGap = 15f;
        private const float MainSettingsHeight = 7 * 32f;
        private const float OwnersTitleHeight = 24f;
        private const float OwnersWindowHeight = 200f;
        private const float WarningHeight = 24f;
        private const float Spacing = 2f;
        private const float ButtonWidth = 60f;

        private bool locked;
        private bool vistitorsAllowed;
        private bool petDoor;
        private bool pensDoor;
        private bool slaveDoor;
        private bool childLock;
        private bool animalsAllowed;

        private Thing lastSelectedThing;

        public ITab_Lock()
        {
            this.size = ITab_Lock.WinSize;
            this.labelKey = "Locks_Label";
            this.tutorTag = "Locks";
        }

        public ThingWithComps SelDoor
        {
            get
            {
                return SelThing as ThingWithComps;
            }
        }

        public LockData Data
        {
            get
            {
                return LockUtility.GetData(SelDoor);
            }
        }

        protected override void FillTab()
        {
            Rect mainRect = new Rect(0f, 0f, ITab_Lock.WinSize.x, ITab_Lock.WinSize.y).ContractedBy(WindowGap);
            Rect upperSettingsRect = new Rect(0f, 0f + WindowGap, mainRect.width, MainSettingsHeight);
            Rect ownersTitleRect = new Rect(0f, mainRect.height - OwnersWindowHeight - OwnersTitleHeight - WarningHeight - 2 * Spacing - ButtonsHeight, mainRect.width, OwnersTitleHeight);
            Rect ownersListRect = new Rect(0f, mainRect.height - OwnersWindowHeight - WarningHeight - 2 * Spacing - ButtonsHeight, mainRect.width, OwnersWindowHeight);
            Rect warningRect = new Rect(0f, mainRect.height - WarningHeight - Spacing - ButtonsHeight, mainRect.width, WarningHeight);
            Rect cancelButtonRect = new Rect(mainRect.width - ButtonWidth, mainRect.height - WarningHeight, ButtonWidth, WarningHeight);
            Rect copyButtonsRect = new Rect(mainRect.width / 2 - Spacing - ButtonWidth, mainRect.height - ButtonsHeight, ButtonWidth, ButtonsHeight);
            Rect pasteButtonsRect = new Rect(mainRect.width / 2 + Spacing, mainRect.height - ButtonsHeight, ButtonWidth, ButtonsHeight);
            Text.Font = GameFont.Small;
            float viewRectCalcHeight = (Text.LineHeight + Spacing) * SelDoor.Map.mapPawns.FreeColonists.Count();
            Rect viewRect = new Rect(0f, 0f, mainRect.width - 16f, viewRectCalcHeight >= OwnersWindowHeight ? viewRectCalcHeight : OwnersWindowHeight);
            Text.Font = GameFont.Small;
            var listing = new Listing_Standard(GameFont.Small);
            GUI.BeginGroup(mainRect);

            // Upper rect
            listing.Begin(upperSettingsRect);
            if (Data.WantedState.IsVisible(nameof(LockState.locked)))
                listing.CheckboxLabeled("Locks_ITabLocked".Translate(), ref locked, "Locks_ITabLockedDesc".Translate());
            if (Data.WantedState.IsVisible(nameof(LockState.allowSlave)))
            {
                listing.CheckboxLabeled("Locks_ITabSlaveDoors".Translate(), ref slaveDoor, "Locks_ITabSlaveDoorsDesc".Translate());
            }
            if (Data.WantedState.IsVisible(nameof(LockState.mode)))
                listing.CheckboxLabeled("Locks_ITabVisitorsAllowed".Translate(), ref vistitorsAllowed, "Locks_ITabVisitorsAllowedDesc".Translate());
            if (Data.WantedState.IsVisible(nameof(LockState.childLock)))
            {
                listing.CheckboxLabeled("Locks_ITabChildrenLock".Translate(), ref childLock, "Locks_ITabChildrenLockDesc".Translate());
            }

            if (Data.WantedState.IsVisible(nameof(LockState.petDoor)))
                listing.CheckboxLabeled("Locks_ITabPetDoor".Translate(), ref petDoor, "Locks_ITabPetDoorDesc".Translate());
            if (Data.WantedState.IsVisible(nameof(LockState.pensDoor)))
                listing.CheckboxLabeled("Locks_ITabPensDoor".Translate(), ref pensDoor, "Locks_ITabPensDoorDesc".Translate());
            if (Data.WantedState.IsVisible(nameof(LockState.allowAnimals)))
            {
                listing.CheckboxLabeled("Locks_ITabAnimalAllowed".Translate(), ref animalsAllowed, "Locks_ITabAnimalAllowedDesc".Translate());
            }

            listing.End();

            if (Data.WantedState.IsVisible(nameof(LockState.owners)))
            {
                // Owners title rect
                Widgets.Label(ownersTitleRect, $"{"Locks_ITabOwners".Translate()}:");

                // Owners list rect
                Widgets.DrawBoxSolid(ownersListRect, new Color(0.2f, 0.2f, 0.2f));
                Widgets.BeginScrollView(ownersListRect, ref scrollPosition, viewRect);
                float curHeight = 0f;
                foreach (var pawn in SelThing.Map.mapPawns.FreeColonists)
                {
                    //Widgets.Label(new Rect(0f, curHeight, viewRect.width, Text.LineHeight), $"{pawn.Name}");
                    OwnerCheckbox(new Rect(0f, curHeight, viewRect.width, Text.LineHeight), pawn);
                    curHeight += Text.LineHeight + Spacing;
                }
                Widgets.EndScrollView();
            }

            if (Data.NeedChange)
            {
                // Notification
                GUI.color = Color.red;
                Widgets.Label(warningRect, "Locks_ITabChangeLocksNotification".Translate());

                // Cancel button
                GUI.color = Color.white;
                if (Widgets.ButtonText(cancelButtonRect, "Locks_Cancel".Translate()))
                {
                    Data.WantedState.CopyFrom(Data.CurrentState);
                    OnOpen();
                }

            }

            // Copy Paste
            if (Widgets.ButtonText(copyButtonsRect, "Locks_Copy".Translate()))
            {
                Clipboard.StoredState = Data.WantedState;
            }
            if (Clipboard.StoredState.HasValue && Widgets.ButtonText(pasteButtonsRect, "Locks_Paste".Translate()))
            {
                Data.WantedState.CopyFrom(Clipboard.StoredState.Value);
                OnOpen();
            }

            //TODO
            //PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.StorageTab, KnowledgeAmount.FrameDisplayed);
            GUI.EndGroup();

            UpdateSettings();
        }

        private void UpdateSettings()
        {
            if (SelDoor == null)
                return;
            if (lastSelectedThing == SelThing)
            {
                Data.WantedState.locked = locked;
                Data.WantedState.mode = vistitorsAllowed ? LockMode.Allies : LockMode.Colony;
                Data.WantedState.petDoor = petDoor;
                Data.WantedState.pensDoor = pensDoor;
                Data.WantedState.allowSlave = slaveDoor;
                Data.WantedState.allowAnimals = animalsAllowed;
                Data.WantedState.childLock = childLock;
                if (Data.NeedChange)
                    LockUtility.UpdateLockDesignation(SelDoor);
            }
            else
            {
                OnOpen();
                lastSelectedThing = SelThing;
            }
        }

        public override void OnOpen()
        {
            locked = Data.WantedState.locked;
            vistitorsAllowed = Data.WantedState.mode == LockMode.Allies;
            petDoor = Data.WantedState.petDoor;
            pensDoor = Data.WantedState.pensDoor;
            slaveDoor = Data.WantedState.allowSlave;
            animalsAllowed = Data.WantedState.allowAnimals;
            childLock = Data.WantedState.childLock;
        }

        private void OwnerCheckbox(Rect rect, Pawn pawn)
        {
            bool checkOn = Data.WantedState.owners.Contains(pawn);
            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect, pawn.Name.ToStringShort);
            if (Widgets.ButtonInvisible(rect, false))
            {
                if (checkOn)
                    Data.WantedState.owners.Remove(pawn);
                else
                    Data.WantedState.owners.Add(pawn);
                checkOn = !checkOn;
                if (checkOn)
                {
                    SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
                }
                else
                {
                    SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
                }
            }
            Color color = GUI.color;
            Texture2D image;
            if (checkOn)
            {
                image = Widgets.CheckboxOnTex;
            }
            else
            {
                image = Widgets.CheckboxOffTex;
            }
            Rect position = new Rect(rect.x + rect.width - 24f, rect.y, 24f, 24f);
            GUI.DrawTexture(position, image);
            Text.Anchor = anchor;
        }
    }
}
