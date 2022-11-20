using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Multiplayer.API;
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
      bool anythingChanged = false;
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
        CheckboxLabeled(listing, ref anythingChanged, "Locks_ITabLocked".Translate(), ref locked, "Locks_ITabLockedDesc".Translate());
      if (Data.WantedState.IsVisible(nameof(LockState.allowSlave)))
      {
        CheckboxLabeled(listing, ref anythingChanged, "Locks_ITabSlaveDoors".Translate(), ref slaveDoor, "Locks_ITabSlaveDoorsDesc".Translate());
      }
      if (Data.WantedState.IsVisible(nameof(LockState.mode)))
        CheckboxLabeled(listing, ref anythingChanged, "Locks_ITabVisitorsAllowed".Translate(), ref vistitorsAllowed, "Locks_ITabVisitorsAllowedDesc".Translate());
      if (Data.WantedState.IsVisible(nameof(LockState.childLock)))
      {
        CheckboxLabeled(listing, ref anythingChanged, "Locks_ITabChildrenLock".Translate(), ref childLock, "Locks_ITabChildrenLockDesc".Translate());
      }

      if (Data.WantedState.IsVisible(nameof(LockState.petDoor)))
      {
        CheckboxLabeled(listing, ref anythingChanged, "Locks_ITabPetDoor".Translate(), ref petDoor, "Locks_ITabPetDoorDesc".Translate());
        if (anythingChanged && petDoor && !animalsAllowed)
        {
          animalsAllowed = true;
        }
      }
      if (Data.WantedState.IsVisible(nameof(LockState.pensDoor)))
      {
        CheckboxLabeled(listing, ref anythingChanged, "Locks_ITabPensDoor".Translate(), ref pensDoor, "Locks_ITabPensDoorDesc".Translate());
        if (anythingChanged && pensDoor && !animalsAllowed)
        {
          animalsAllowed = true;
        }
      }
      if (Data.WantedState.IsVisible(nameof(LockState.allowAnimals)))
      {
        CheckboxLabeled(listing, ref anythingChanged, "Locks_ITabAnimalAllowed".Translate(), ref animalsAllowed, "Locks_ITabAnimalAllowedDesc".Translate());
        if (anythingChanged && !animalsAllowed)
        {
          pensDoor = false;
          petDoor = false;
        }
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
          OwnerCheckbox(new Rect(0f, curHeight, viewRect.width, Text.LineHeight), pawn, ref anythingChanged);
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
          SetWantedStateData(SelDoor, Data.CurrentState);
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
        SetWantedStateData(SelDoor, Clipboard.StoredState.Value);
        OnOpen();
      }

      //TODO
      //PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.StorageTab, KnowledgeAmount.FrameDisplayed);
      GUI.EndGroup();

      UpdateSettings(anythingChanged);
    }

    private void UpdateSettings(bool anythingChanged)
    {
      if (SelDoor == null)
        return;
      if (lastSelectedThing == SelThing)
      {
        if (anythingChanged)
        {
          var newState = new LockState(vistitorsAllowed ? LockMode.Allies : LockMode.Colony, locked, petDoor, pensDoor, null, slaveDoor, animalsAllowed)
          {
            childLock = childLock
          };
          SetWantedStateData(SelDoor, newState);
        }
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

    private void OwnerCheckbox(Rect rect, Pawn pawn, ref bool anythingChanged)
    {
      bool checkOn = Data.WantedState.owners.Contains(pawn);
      TextAnchor anchor = Text.Anchor;
      Text.Anchor = TextAnchor.MiddleLeft;
      Widgets.Label(rect, pawn.Name.ToStringShort);
      if (Widgets.ButtonInvisible(rect, false))
      {
        SetOwnerWantedState(SelDoor, pawn, checkOn);
        checkOn = !checkOn;
        anythingChanged = true;
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

    private static void CheckboxLabeled(Listing_Standard listing, ref bool anythingChanged, string label, ref bool checkOn, string tooltip = null, float height = 0.0f, float labelPct = 1f)
    {
      var current = checkOn;
      listing.CheckboxLabeled(label, ref checkOn, tooltip, height, labelPct);
      if (current != checkOn)
        anythingChanged = true;
    }

    /**
    * Doors in argument insted SelDoor and Data needed for MP
    */
    [SyncMethod(SyncContext.MapSelected)]
    private static void SetWantedStateData(ThingWithComps door, LockState newState)
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

    /**
     * Doors in argument insted SelDoor and Data needed for MP
     */
    [SyncMethod]
    private static void SetOwnerWantedState(ThingWithComps door, Pawn pawn, bool checkOn)
    {
      var data = LockUtility.GetData(door);

      if (checkOn)
        data.WantedState.owners.Remove(pawn);
      else
        data.WantedState.owners.Add(pawn);
    }
  }
}
