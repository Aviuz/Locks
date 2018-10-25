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

        private static readonly Vector2 WinSize = new Vector2(285f + WindowGap, PreGap + OwnersWindowHeight + 94f + WindowGap + 24f);
        private const float PreGap = 10f;
        private const float WindowGap = 15f;
        private const float OwnersWindowHeight = 200f;
        private const float Spacing = 2f;

        private bool locked;
        private bool vistitorsAllowed;
        private bool petDoor;

        private Thing lastSelectedThing;

        public ITab_Lock()
        {
            this.size = ITab_Lock.WinSize;
            this.labelKey = "Locks_Label";
            this.tutorTag = "Locks";
        }

        private Building SelDoor => SelThing as Building;
        private CompLock Data => SelDoor.GetComp<CompLock>();

        protected override void FillTab()
        {
            Rect mainRect = new Rect(0f, PreGap, ITab_Lock.WinSize.x, ITab_Lock.WinSize.y - PreGap).ContractedBy(WindowGap);
            Rect upperRect = new Rect(0f, 0f, mainRect.width, mainRect.height - OwnersWindowHeight - 24f);
            Rect bottomRect = new Rect(0f, mainRect.height - OwnersWindowHeight - 24f, mainRect.width, OwnersWindowHeight);
            float viewRectCalcHeight = (Text.LineHeight + Spacing) * SelDoor.Map.mapPawns.FreeColonists.Count();
            Rect viewRect = new Rect(0f, 0f, mainRect.width - 16f, viewRectCalcHeight >= OwnersWindowHeight ? viewRectCalcHeight : OwnersWindowHeight);
            Text.Font = GameFont.Small;
            var listing = new Listing_Standard(GameFont.Small);
            GUI.BeginGroup(mainRect);

            // Upper rect
            listing.Begin(upperRect);
            listing.CheckboxLabeled("Locks_ITabLocked".Translate(), ref locked, "Locks_ITabLockedDesc".Translate());
            listing.CheckboxLabeled("Locks_ITabVisitorsAllowed".Translate(), ref vistitorsAllowed, "Locks_ITabVisitorsAllowedDesc".Translate());
            listing.CheckboxLabeled("Locks_ITabPetDoor".Translate(), ref petDoor, "Locks_ITabPetDoorDesc".Translate());
            listing.Label($"{"Locks_ITabOwners".Translate()}:");
            listing.End();

            // Bottom rect
            Widgets.BeginScrollView(bottomRect, ref scrollPosition, viewRect);
            float curHeight = 0f;
            Widgets.DrawBoxSolid(viewRect, new Color(0.2f, 0.2f, 0.2f));
            foreach (var pawn in SelThing.Map.mapPawns.FreeColonists)
            {
                //Widgets.Label(new Rect(0f, curHeight, viewRect.width, Text.LineHeight), $"{pawn.Name}");
                OwnerCheckbox(new Rect(0f, curHeight, viewRect.width, Text.LineHeight), pawn);
                curHeight += Text.LineHeight + Spacing;
            }
            Widgets.EndScrollView();

            // Notification
            if (Data.NeedChange)
            {
                GUI.color = Color.red;
                Widgets.Label(new Rect(0f, mainRect.height - 24f, mainRect.width, 24f), "Locks_ITabChangeLocksNotification".Translate());
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
                Data.wantedState.locked = locked;
                Data.wantedState.mode = vistitorsAllowed ? LockMode.Allies : LockMode.Colony;
                Data.wantedState.petDoor = petDoor;
                // Owners
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
            locked = Data.wantedState.locked;
            vistitorsAllowed = Data.wantedState.mode == LockMode.Allies ? true : false;
            petDoor = Data.wantedState.petDoor;
            //owners
        }

        private void OwnerCheckbox(Rect rect, Pawn pawn)
        {
            bool checkOn = Data.wantedState.owners.Contains(pawn);
            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect, pawn.Name.ToStringShort);
            if (Widgets.ButtonInvisible(rect, false))
            {
                if (checkOn)
                    Data.wantedState.owners.Remove(pawn);
                else
                    Data.wantedState.owners.Add(pawn);
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
