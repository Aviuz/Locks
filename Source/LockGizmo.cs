using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Locks
{
    class LockGizmo : Command
    {
        private Building_Door parent;

        private Texture2D lockTexture;
        private Texture2D unlockTexture;

        private Func<bool> isActive;

        public LockGizmo(Building_Door door)
        {
            parent = door;
            defaultLabel = "Locks_Label".Translate();
            defaultDesc = "Locks_Description".Translate();
            lockTexture = ContentFinder<Texture2D>.Get("lock", false);
            unlockTexture = ContentFinder<Texture2D>.Get("unlock", false);
            isActive = () => LockUtility.GetData(parent).WantedState.locked;
        }

        public override void ProcessInput(Event ev)
        {
            if (ev.button == 0)
            {
                SoundDefOf.Click.PlayOneShotOnCamera(null);
                LockUtility.GetData(parent).WantedState.locked = !LockUtility.GetData(parent).WantedState.locked;
                LockUtility.UpdateLockDesignation(parent);
            }
            else if (ev.button == 1)
            {
                SoundDefOf.Click.PlayOneShotOnCamera(null);
                var floatMenu = new FloatMenu(GetMenuOptions());
                Find.WindowStack.Add(floatMenu);
            }
        }

        public List<FloatMenuOption> GetMenuOptions()
        {
            var list = new List<FloatMenuOption>();
            list.Add(new FloatMenuOption(
                LockUtility.GetData(parent).WantedState.locked ?
                                        "Locks_UnlockToggle".Translate() :
                                        "Locks_LockToggle".Translate(),
                new Action(() =>
                {
                    bool value = !LockUtility.GetData(parent).WantedState.locked;
                    foreach (Building_Door door in Find.Selector.SelectedObjects.Where(o => o is Building_Door))
                    {
                        LockUtility.GetData(door).WantedState.locked = value;
                        LockUtility.UpdateLockDesignation(door);
                    }
                })
                ));
            if (LockUtility.GetData(parent).WantedState.IsVisible(nameof(LockState.mode)))
            {
                list.Add(new FloatMenuOption(
                    LockUtility.GetData(parent).WantedState.mode == LockMode.Allies ?
                                        "Locks_ForbidVisitors".Translate() :
                                        "Locks_AllowVisitors".Translate(),
                    new Action(() =>
                    {
                        LockMode value;
                        if (LockUtility.GetData(parent).WantedState.mode == LockMode.Allies)
                            value = LockMode.Colony;
                        else
                            value = LockMode.Allies;
                        foreach (Building_Door door in Find.Selector.SelectedObjects.Where(o => o is Building_Door))
                        {
                            LockUtility.GetData(door).WantedState.mode = value;
                            LockUtility.UpdateLockDesignation(door);
                        }
                    })
                    ));
            }
            if (LockUtility.GetData(parent).WantedState.IsVisible(nameof(LockState.petDoor)))
            {
                list.Add(new FloatMenuOption(
                    LockUtility.GetData(parent).WantedState.petDoor ?
                                            "Locks_RemovePetDoor".Translate() :
                                            "Locks_AddPetDoor".Translate(),
                    new Action(() =>
                    {
                        bool value = !LockUtility.GetData(parent).WantedState.petDoor;
                        foreach (Building_Door door in Find.Selector.SelectedObjects.Where(o => o is Building_Door))
                        {
                            LockUtility.GetData(door).WantedState.petDoor = value;
                            LockUtility.UpdateLockDesignation(door);
                        }
                    })
                    ));
            }
            if (LockUtility.GetData(parent).WantedState.IsVisible(nameof(LockState.owners)))
            {
                list.Add(new FloatMenuOption(
                    "CommandBedSetOwnerLabel".Translate(),
                    new Action(() =>
                    {
                        Find.WindowStack.Add(new Dialog_AssignBuildingOwner(LockUtility.GetData(parent).CompAssignableToPawn));
                        foreach (Building_Door door in Find.Selector.SelectedObjects.Where(o => o is Building_Door))
                        {
                            if (door != parent)
                            {
                                LockUtility.GetData(door).WantedState.owners.Clear();
                                LockUtility.GetData(door).WantedState.owners.AddRange(LockUtility.GetData(parent).WantedState.owners);
                            }
                            LockUtility.UpdateLockDesignation(door);
                        }
                    })
                    ));
            }
            if (LockUtility.GetData(parent).WantedState.Private)
            {
                list.Add(new FloatMenuOption(
                    "Locks_ClearOwners".Translate(),
                    new Action(() =>
                    {
                        foreach (Building_Door door in Find.Selector.SelectedObjects.Where(o => o is Building_Door))
                        {
                            LockUtility.GetData(door).WantedState.owners.Clear();
                            LockUtility.UpdateLockDesignation(door);
                        }
                    })
                    ));
            }
            return list;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
            Rect rect = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
            bool flag = false;
            if (Mouse.IsOver(rect))
            {
                flag = true;
                GUI.color = GenUI.MouseoverColor;
            }
            Texture2D badTex = LockUtility.GetData(parent).WantedState.locked ? lockTexture : unlockTexture;
            if (badTex == null)
            {
                badTex = BaseContent.BadTex;
            }
            GUI.DrawTexture(rect, BGTex);
            GUI.DrawTexture(rect, badTex);
            MouseoverSounds.DoRegion(rect, SoundDefOf.Mouseover_Command);
            bool flag2 = false;
            KeyCode keyCode = (this.hotKey != null) ? hotKey.MainKey : KeyCode.None;
            if (keyCode != KeyCode.None && !GizmoGridDrawer.drawnHotKeys.Contains(keyCode))
            {
                Rect rect2 = new Rect(rect.x + 5f, rect.y + 5f, rect.width - 10f, 18f);
                Widgets.Label(rect2, keyCode.ToStringReadable());
                GizmoGridDrawer.drawnHotKeys.Add(keyCode);
                if (hotKey.KeyDownEvent)
                {
                    flag2 = true;
                    Event.current.Use();
                }
            }
            if (Widgets.ButtonInvisible(rect, false))
            {
                flag2 = true;
            }
            string labelCap = LabelCap;
            if (!labelCap.NullOrEmpty())
            {
                float num = Text.CalcHeight(labelCap, rect.width);
                Rect rect3 = new Rect(rect.x, rect.yMax - num + 12f, rect.width, num);
                GUI.DrawTexture(rect3, TexUI.GrayTextBG);
                GUI.color = Color.white;
                Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label(rect3, labelCap);
                Text.Anchor = TextAnchor.UpperLeft;
                GUI.color = Color.white;
            }
            GUI.color = Color.white;
            if (DoTooltip)
            {
                TipSignal tip = Desc;
                if (disabled && !disabledReason.NullOrEmpty())
                {
                    string text = tip.text;
                    tip.text = string.Concat(new string[]
                    {
                text,
                "\n\n",
                "DisabledCommand".Translate(),
                ": ",
                disabledReason
                    });
                }
                TooltipHandler.TipRegion(rect, tip);
            }
            if (!HighlightTag.NullOrEmpty() && (Find.WindowStack.FloatMenu == null || !Find.WindowStack.FloatMenu.windowRect.Overlaps(rect)))
            {
                UIHighlighter.HighlightOpportunity(rect, HighlightTag);
            }
            if (flag2)
            {
                if (disabled)
                {
                    if (!disabledReason.NullOrEmpty())
                    {
                        Messages.Message(disabledReason, MessageTypeDefOf.RejectInput);
                    }
                    return new GizmoResult(GizmoState.Mouseover, null);
                }
                if (!TutorSystem.AllowAction(TutorTagSelect))
                {
                    return new GizmoResult(GizmoState.Mouseover, null);
                }
                GizmoResult result = new GizmoResult(GizmoState.Interacted, Event.current);
                TutorSystem.Notify_Event(TutorTagSelect);
                return result;
            }
            else
            {
                if (flag)
                {
                    return new GizmoResult(GizmoState.Mouseover, null);
                }
                return new GizmoResult(GizmoState.Clear, null);
            }
        }

        public override bool InheritInteractionsFrom(Gizmo other)
        {
            LockGizmo lockGizmo = other as LockGizmo;
            if (lockGizmo != null)
                return lockGizmo.isActive() == isActive();
            return false;
        }
    }
}
