using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Locks
{
  public class LockOptionsDialog : Window
  {
    private const float TextRow = 40f;
    private const float PaddingLeft = 10f;
    private const float Spacing = 4f;
    private readonly ThingWithComps door;

    private readonly List<PawnKindDef> pawnKinds = LockUtility.MechKinds;
    private readonly QuickSearchWidget quickSearchWidget = new QuickSearchWidget();
    private readonly Dictionary<int, bool> selectedChoices = new Dictionary<int, bool>();
    private int index;
    private LockState lockData;
    private Vector2 scrollPosition;

    public LockOptionsDialog(ThingWithComps door)
    {
      this.door = door;
      lockData = new LockState();
      lockData.CopyFrom(LockUtility.GetData(door).WantedState);
    }

    private DoorAllowed Data => index == 1 ? lockData.ColonistDoor : lockData.SlaveAllowed;

    private List<Pawn> PawnsList => index == 1
      ? GetFreeNonSlaveColonist()
      : Find.CurrentMap.mapPawns.SlavesOfColonySpawned;

    private static List<Pawn> GetFreeNonSlaveColonist()
    {
      return Find.CurrentMap.mapPawns.FreeColonists.Where(pawn => pawn.IsFreeNonSlaveColonist).ToList();
    }

    public override void DoWindowContents(Rect inRect)
    {
      Text.Anchor = TextAnchor.MiddleCenter;
      var menu = new Rect(0, 0, 100, inRect.height - 60f);
      DrawOptionsSelections(menu);

      var body = new Rect(104, 0, inRect.width - 100 - 4, inRect.height - 60f);
      Text.Anchor = TextAnchor.MiddleLeft;
      DrawBody(body);

      DrawBottomButtons(inRect);
      Text.Anchor = TextAnchor.UpperLeft;
    }

    private void DrawBottomButtons(Rect inRect)
    {
      if (lockData != LockUtility.GetData(door).WantedState)
      {
        var updateButton = new Rect(inRect.width * 0.25f, inRect.height - 50f, inRect.width * 0.25f, 50f);
        if (Widgets.ButtonText(updateButton, "Locks_UpdateButton".Translate()))
        {
          CopyUtils.SetWantedStateData(door, lockData);
          Close();
        }

        var closeMenu = new Rect(inRect.width * 0.5f, inRect.height - 50f, inRect.width * 0.25f, 50f);
        if (Widgets.ButtonText(closeMenu, "Locks_CancelButton".Translate()))
        {
          Close();
        }
      }
      else
      {
        var closeMenu = new Rect(inRect.width * 0.25f, inRect.height - 50f, inRect.width * 0.5f, 50f);
        if (Widgets.ButtonText(closeMenu, "Locks_CancelButton".Translate()))
        {
          Close();
        }
      }
    }

    private void DrawBody(Rect body)
    {
      var buttonRect = new Rect(body.x + PaddingLeft, body.y, body.width - PaddingLeft, TextRow);
      switch (index)
      {
        case 0:
          DrawGeneral(body);
          break;
        case 1:
        case 2:
          Widgets.Label(new Rect(body.x + PaddingLeft, body.y, buttonRect.width / 2f, TextRow),
            "Locks_Allowed".Translate());

          if (Widgets.ButtonText(buttonRect.Indent(buttonRect.width / 2f),
                GetLabel(Data, GetSelectedChoice(index)).Translate()))
          {
            var options = new List<FloatMenuOption>
            {
              new FloatMenuOption("Locks_Any".Translate(), delegate { ModifyData(true, false); }),
              new FloatMenuOption("Locks_None".Translate(), delegate { ModifyData(false, false); }),
              new FloatMenuOption("Locks_Selected".Translate(), delegate { ModifyData(false, true); })
            };
            Find.WindowStack.Add(new FloatMenu(options));
          }

          if (GetSelectedChoice(index) || Data.AllowedPawns?.Count > 0)
          {
            DrawPawnList(
              DrawSearchWidget(new Rect(body.x, body.y + TextRow, body.width,
                body.height - TextRow)), PawnsList, Data.AllowedPawns);
          }

          break;
        case 3:
          Widgets.Label(new Rect(body.x + PaddingLeft, body.y, buttonRect.width / 2f, TextRow),
            "Locks_Allowed".Translate());

          if (Widgets.ButtonText(buttonRect.Indent(buttonRect.width / 2f),
                GetMechLabel(GetSelectedChoice(index)).Translate()))
          {
            var options = new List<FloatMenuOption>
            {
              new FloatMenuOption("Locks_Any".Translate(), delegate { ModifyMechData(true, false, false); }),
              new FloatMenuOption("Locks_None".Translate(), delegate { ModifyMechData(false, false, false); }),
              new FloatMenuOption("Locks_MechanitorsOwned".Translate(),
                delegate { ModifyMechData(false, true, false); }),
              new FloatMenuOption("Locks_Selected".Translate(), delegate { ModifyMechData(false, false, true); })
            };
            Find.WindowStack.Add(new FloatMenu(options));
          }

          if (GetSelectedChoice(index) || lockData.MechanoidDoor.AllowedMechanoids?.Count > 0)
          {
            DrawMechs(DrawSearchWidget(new Rect(body.x, body.y + TextRow, body.width, body.height - TextRow)));
          }

          break;
      }
    }

    private void ModifyMechData(bool any, bool owned, bool selected)
    {
      lockData.MechanoidDoor.Any = any;
      lockData.MechanoidDoor.OnlyMechanitorsMechs = owned;
      selectedChoices[index] = selected;
      if (!selected)
      {
        lockData.MechanoidDoor.AllowedMechanoids?.Clear();
      }
    }

    private void ModifyData(bool any, bool selected)
    {
      if (index == 1)
      {
        lockData.ColonistDoor.Any = any;
        if (!selected && !any)
        {
          lockData.ColonistDoor.AllowedPawns?.Clear();
        }
      }
      else
      {
        lockData.SlaveAllowed.Any = any;
        if (!selected && !any)
        {
          lockData.SlaveAllowed.AllowedPawns?.Clear();
        }
      }

      selectedChoices[index] = selected;
    }

    private bool GetSelectedChoice(int key)
    {
      return selectedChoices.ContainsKey(key) && selectedChoices[key];
    }

    private static string GetLabel(DoorAllowed data, bool selectsChoice)
    {
      if (selectsChoice || data.AllowedPawns?.Count > 0)
      {
        return "Locks_Selected";
      }

      return data.Any ? "Locks_Any" : "Locks_None";
    }

    private string GetMechLabel(bool selectsChoice)
    {
      if (selectsChoice || lockData.MechanoidDoor.AllowedMechanoids?.Count > 0)
      {
        return "Locks_Selected";
      }

      if (lockData.MechanoidDoor.OnlyMechanitorsMechs)
      {
        return "Locks_MechanitorsOwned";
      }

      return lockData.MechanoidDoor.Any ? "Locks_Any" : "Locks_None";
    }

    private void DrawMechs(Rect body)
    {
      var rowHeight = 60f;
      var startX = body.x + PaddingLeft;
      var rect = new Rect(startX, body.y + PaddingLeft, body.width * 0.8f, rowHeight);
      Widgets.BeginScrollView(
        new Rect(startX, body.y + PaddingLeft, body.width - PaddingLeft, body.height - PaddingLeft * 3f),
        ref scrollPosition,
        new Rect(startX, body.y + PaddingLeft, body.width * 0.8f, pawnKinds.Count * rowHeight));

      foreach (var mech in pawnKinds)
      {
        var graphic = mech.lifeStages[0].bodyGraphicData.Graphic.MatEast.mainTexture as Texture2D;
        var customRow = new CustomRow(rect);
        var defName = mech.defName;
        customRow.DrawRadioBox(lockData.MechanoidDoor.AllowedMechanoids.Contains(defName));
        customRow.DrawTexture(graphic, new Vector2(rowHeight, rowHeight));
        customRow.DrawLabel(mech.LabelCap);
        if (customRow.ButtonInvisible())
        {
          if (lockData.MechanoidDoor.AllowedMechanoids.Contains(defName))
          {
            lockData.MechanoidDoor.AllowedMechanoids.Remove(defName);
          }
          else
          {
            lockData.MechanoidDoor.AllowedMechanoids.Add(defName);
          }
        }

        rect = rect.Row(rowHeight);
      }

      Widgets.EndScrollView();
    }

    private void DrawGeneral(Rect body)
    {
      var rowHeight = 40f;
      var actualRow = new Rect(body.x + PaddingLeft, 0, body.width - PaddingLeft, rowHeight);


      DoRowWithButton(actualRow, "Locked", ref lockData.Locked);
      actualRow = actualRow.Row(rowHeight);
      if (lockData.Locked)
      {
        var allies = lockData.Mode == LockMode.Allies;
        if (DoRowWithButton(actualRow.Indent(Widgets.RadioButtonSize), "Locks_VisitorsAllowed".Translate(), ref allies))
        {
          lockData.Mode = allies ? LockMode.Allies : LockMode.Colony;
        }

        actualRow = actualRow.Row(rowHeight);

        DoRowWithButton(actualRow.Indent(Widgets.RadioButtonSize), "Locks_ITabChildrenLock".Translate(),
          ref lockData.ChildLock);
        actualRow = actualRow.Row(rowHeight);
      }

      Widgets.Label(actualRow, "Locks_AnimalMode".Translate());
      if (Widgets.ButtonText(actualRow.Indent(actualRow.width / 2f),
            GetAnimalLabel().Translate()))
      {
        var options = new List<FloatMenuOption>
        {
          new FloatMenuOption("Locks_Any".Translate(), delegate { ModifyAnimalsData(true, false, false); }),
          new FloatMenuOption("Locks_None".Translate(), delegate { ModifyAnimalsData(false, false, false); }),
          new FloatMenuOption("Locks_PetDoor".Translate(),
            delegate { ModifyAnimalsData(true, true, false); }),
          new FloatMenuOption("Locks_PensDoor".Translate(), delegate { ModifyAnimalsData(true, false, true); })
        };
        Find.WindowStack.Add(new FloatMenu(options));
      }
    }

    private string GetAnimalLabel()
    {
      if (lockData.AnimalDoor.PensDoor)
      {
        return "Locks_PensDoor";
      }

      if (lockData.AnimalDoor.OnlyPets)
      {
        return "Locks_PetDoor";
      }

      return lockData.AnimalDoor.Allowed ? "Locks_Any" : "Locks_None";
    }

    private void ModifyAnimalsData(bool allowed, bool onlyPets, bool onlyPen)
    {
      lockData.AnimalDoor.Allowed = allowed;
      lockData.AnimalDoor.PensDoor = onlyPen;
      lockData.AnimalDoor.OnlyPets = onlyPets;
    }

    private static bool DoRowWithButton(Rect row, string label, ref bool checkedOut)
    {
      var customRow = new CustomRow(row);
      customRow.DrawRadioBox(checkedOut);
      customRow.DrawLabel(label);
      return customRow.DrawButtonInvisible(ref checkedOut);
    }

    private void DrawOptionsSelections(Rect rect)
    {
      var actualRow = new Rect(0f, 0f, rect.width, 48f);
      foreach (var entity in GetOptions())
      {
        DoCategoryRow(actualRow.ContractedBy(4f), entity);
        actualRow = actualRow.Row(50f);
      }
    }

    private void DoCategoryRow(Rect r, KeyValuePair<string, int> entity)
    {
      Widgets.DrawOptionBackground(r, index == entity.Value);
      if (Widgets.ButtonInvisible(r))
      {
        index = entity.Value;
        quickSearchWidget.Reset();
        SoundDefOf.Click.PlayOneShotOnCamera();
      }

      var num = r.x + PaddingLeft;
      Widgets.Label(new Rect(num, r.y, r.width - num, r.height), entity.Key.Translate());
    }

    private Dictionary<string, int> GetOptions()
    {
      var ints = new Dictionary<string, int>
      {
        { "Locks_GeneralRow", 0 }
      };
      if (lockData.Locked)
      {
        ints.Add("Locks_ColonistRow", 1);
      }

      if (ModsConfig.IdeologyActive && lockData.Locked)
      {
        ints.Add("Locks_SlavesRow", 2);
      }

      if (ModsConfig.BiotechActive && lockData.Locked)
      {
        ints.Add("Locks_MechsRow", 3);
      }

      return ints;
    }

    private Rect DrawSearchWidget(Rect body)
    {
      var startX = body.x + PaddingLeft;
      quickSearchWidget.OnGUI(new Rect(startX, body.y, body.width - PaddingLeft, QuickSearchWidget.WidgetHeight));
      return new Rect(body.x, body.y + QuickSearchWidget.WidgetHeight, body.width,
        body.height - QuickSearchWidget.WidgetHeight);
    }

    private void DrawPawnList(Rect body, List<Pawn> pawns, List<Pawn> selectedPawns)
    {
      var heightInc = 80f;
      var startX = body.x + PaddingLeft;
      Widgets.BeginScrollView(
        new Rect(startX, body.y + PaddingLeft, body.width - PaddingLeft, body.height - PaddingLeft * 3f),
        ref scrollPosition,
        new Rect(startX, body.y + PaddingLeft, body.width * 0.8f, pawns.Count * heightInc));
      var curHeight = body.y + Spacing;
      foreach (var pawn in GetPawnToShow(pawns))
      {
        var texture = PortraitsCache.Get(pawn, ColonistBarColonistDrawer.PawnTextureSize, Rot4.South);
        var row = new CustomRow(new Rect(startX, curHeight, body.width - PaddingLeft, heightInc));
        if (row.ButtonInvisible())
        {
          if (selectedPawns.Contains(pawn))
          {
            selectedPawns.Remove(pawn);
          }
          else
          {
            selectedPawns.Add(pawn);
          }
        }

        row.DrawRadioBox(selectedPawns.Contains(pawn));
        row.DrawTexture(texture, new Vector2(heightInc, heightInc));
        row.DrawLabel(pawn.NameFullColored.RawText);

        curHeight += heightInc;
      }

      Widgets.EndScrollView();
    }

    private List<Pawn> GetPawnToShow(List<Pawn> pawns)
    {
      return quickSearchWidget.filter.Text.NullOrEmpty()
        ? pawns
        : pawns.FindAll(pawn =>
          pawn.NameFullColored.RawText.IndexOf(quickSearchWidget.filter.Text,
            StringComparison.OrdinalIgnoreCase) > -1);
    }
  }
}