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
    private static readonly Vector2 WinSize = new Vector2(310f + WindowGap,
      ButtonsHeight + MainSettingsHeight + WindowGap + 2 * Spacing + WarningHeight);

    private const float ButtonsHeight = 25f;
    private const float WindowGap = 15f;
    private const float MainSettingsHeight = 7 * 32f;
    private const float WarningHeight = 24f;
    private const float Spacing = 2f;
    private const float ButtonWidth = 60f;

    public ITab_Lock()
    {
      this.size = ITab_Lock.WinSize;
      this.labelKey = "Locks_Label";
      this.tutorTag = "Locks";
    }

    private ThingWithComps SelDoor => SelThing as ThingWithComps;

    private LockData Data => LockUtility.GetData(SelDoor);
    public override bool IsVisible => SelDoor.Faction == Faction.OfPlayer;

    protected override void FillTab()
    {
      Rect mainRect = new Rect(0f, 0f, WinSize.x, WinSize.y).ContractedBy(WindowGap);
      Rect upperSettingsRect = new Rect(0f, 0f + WindowGap, mainRect.width, MainSettingsHeight);
      Rect copyButtonsRect = new Rect(mainRect.width / 2 - Spacing - ButtonWidth * 1.5f,
        mainRect.height - ButtonsHeight, ButtonWidth, ButtonsHeight);
      Rect pasteButtonsRect = new Rect(mainRect.width / 2 + ButtonWidth * 0.5f,
        mainRect.height - ButtonsHeight, ButtonWidth, ButtonsHeight);
      Rect editButtonRect = new Rect(mainRect.width / 2 + Spacing + 0.5f * ButtonWidth, mainRect.height - ButtonsHeight,
        ButtonWidth, ButtonsHeight);
      Text.Font = GameFont.Small;
      var listing = new Listing_Standard(GameFont.Small);
      GUI.BeginGroup(mainRect);
      listing.Begin(upperSettingsRect);
      var state = Data.WantedState;
      listing.Label(state.Locked ? "Locks_Locked".Translate() : "Locks_Unlocked".Translate());
      if (state.Locked)
      {
        listing.Label(state.ChildLock
          ? "Locks_ChildrenLockEnabled".Translate()
          : "Locks_ChildrenLockDisabled".Translate());


        listing.Label(state.Mode == LockMode.Allies ? "Locks_Allies".Translate() : "Locks_ColonyOnly".Translate());
        if (state.ColonistDoor.Any)
        {
          listing.Label("Locks_AnyColonistAllowed".Translate());
        }
        else
        {
          var nonColonist = (state.ColonistDoor.AllowedPawns?.Count ?? 0) == 0;
          if (nonColonist)
          {
            listing.Label("Locks_ColonistNone".Translate());
          }
          else
          {
            var pawnTooltips = String.Join("\n", state.ColonistDoor.AllowedPawns.Select(pawn => pawn.LabelShort));
            listing.Label("Locks_FewColonist".Translate(state.ColonistDoor.AllowedPawns.Count),
              tooltip: "Locks_AllowedColonist".Translate(pawnTooltips));
          }
        }

        if (ModsConfig.IdeologyActive)
        {
          if (state.SlaveAllowed.Any)
          {
            listing.Label("Locks_AnySlaveAllowed".Translate());
          }
          else
          {
            var nonColonist = (state.SlaveAllowed.AllowedPawns?.Count ?? 0) == 0;
            var tooltip = nonColonist
              ? null
              : "Locks_AllowedSlaves".Translate(String.Join("\n",
                state.SlaveAllowed.AllowedPawns.Select(pawn => pawn.LabelShort)));
            listing.Label(nonColonist
              ? "Locks_SlaveNone".Translate()
              : "Locks_FewSlave".Translate(state.SlaveAllowed.AllowedPawns.Count), tooltip: tooltip);
          }
        }

        listing.Label(AnimalLabel());
        if (ModsConfig.BiotechActive)
        {
          if (state.MechanoidDoor.Any)
          {
            listing.Label("Locks_AnyMechanoidAllowed".Translate());
          }
          else
          {
            if (state.MechanoidDoor.OnlyMechanitorsMechs)
            {
              listing.Label("Locks_OnlyMechanitorsMechs".Translate());
            }
            else
            {
              var toolTip = String.Join("\n",
                LockUtility.MechKinds.Select(def => state.MechanoidDoor.AllowedMechanoids.Contains(def.defName)));
              listing.Label("Locks_OnlyAllowedMechs".Translate(state.MechanoidDoor.AllowedMechanoids.Count),
                tooltip: "Locks_AllowedMechsToolTip".Translate(toolTip));
            }
          }
        }
      }
      else
      {
        listing.Label(AnimalLabel());
      }

      listing.End();

      // Copy Paste
      if (Widgets.ButtonText(copyButtonsRect, "Locks_Copy".Translate()))
      {
        Clipboard.StoredState = state;
      }

      if (Clipboard.StoredState.HasValue && Widgets.ButtonText(pasteButtonsRect, "Locks_Paste".Translate()))
      {
        CopyUtils.SetWantedStateData(SelDoor, Clipboard.StoredState.Value);
        OnOpen();
      }

      if (Widgets.ButtonText(editButtonRect, "Locks_Edit".Translate()))
      {
        Find.WindowStack.Add(new LockOptionsDialog(SelDoor));
      }

      //TODO
      //PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.StorageTab, KnowledgeAmount.FrameDisplayed);
      GUI.EndGroup();
    }

    private String AnimalLabel()
    {
      var animalDoor = Data.WantedState.AnimalDoor;
      if (!animalDoor.Allowed)
      {
        return "Locks_NoneAnimalLabel".Translate();
      }

      if (animalDoor.PensDoor)
      {
        return "Locks_PensDoorLabel".Translate();
      }

      return animalDoor.OnlyPets ? "Locks_OnlyPetLabel".Translate() : "Locks_AnyAnimalLabel".Translate();
    }
  }
}