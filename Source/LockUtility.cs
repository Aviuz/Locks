using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace Locks
{
    public static class LockUtility
    {
        public static float MaxPetSize = 0.86f;

        private static readonly Dictionary<Building_Door, LockData> Map = new Dictionary<Building_Door, LockData>();

        private static DesignationDef designationDef;
        private static JobDef jobDef;

        public static DesignationDef DesDef
        {
            get
            {
                if (designationDef == null)
                {
                    designationDef = DefDatabase<DesignationDef>.GetNamed("Locks_Flick");
                }

                return designationDef;
            }
        }

        public static JobDef JobDef
        {
            get
            {
                if (jobDef == null)
                {
                    jobDef = DefDatabase<JobDef>.GetNamed("Locks_Flick");
                }

                return jobDef;
            }
        }

        public static bool PawnCanOpen(Building_Door door, Pawn p)
        {
            Lord lord = p.GetLord();

            bool canOpenAnyDoor = lord != null && lord.LordJob != null && lord.LordJob.CanOpenAnyDoor(p);
            bool noFaction = door.Faction == null;
            bool specialGuest = WildManUtility.WildManShouldReachOutsideNow(p) || (p.guest != null && p.guest.Released);
            if (canOpenAnyDoor || noFaction || specialGuest)
                return true;

            LockState respectedState;
            if (!p.IsPrisoner && !door.Faction.HostileTo(p.Faction) && !p.InMentalState)
                respectedState = GetData(door).WantedState;
            else
                respectedState = GetData(door).CurrentState;

            if (GetData(door).CurrentState.locked == false && p.RaceProps != null && p.RaceProps.intelligence >= Intelligence.Humanlike)
                return true;

            if (p.Faction == null || p.Faction.HostileTo(door.Faction))
                return false;
            Log.Message($"State check: {respectedState.pensDoor}, { p.RaceProps.FenceBlocked} {!door.def.building.roamerCanOpen} {!p.roping.IsRopedByPawn}");
            if(  p.roping.RopedByPawn != null)
            {
                Log.Message($"{!PawnCanOpen(door, p.roping.RopedByPawn)}");
            }
            if (respectedState.pensDoor && p.RaceProps.FenceBlocked && !door.def.building.roamerCanOpen && (!p.roping.IsRopedByPawn || !PawnCanOpen(door, p.roping.RopedByPawn)))
            {
                return false;
            }

            if (respectedState.Private && respectedState.petDoor && p.RaceProps != null && p.RaceProps.Animal && p.RaceProps.baseBodySize <= MaxPetSize && p.Faction == door.Faction)
                return true;

            if (respectedState.Private && !respectedState.owners.Contains(p))
                return false;

            if (p.Faction == door.Faction && !p.IsPrisoner)
                return true;

            bool guestCondition = !p.IsPrisoner || p.HostFaction != door.Faction;
            if (respectedState.mode == LockMode.Allies && guestCondition)
                return true;

            return false;
        }

        public static LockData GetData(Building_Door key)
        {
            if (!Map.ContainsKey(key))
                Map[key] = new LockData();
            Map[key].UpdateReference(key);
            return Map[key];
        }

        public static void Remove(Building_Door key)
        {
            Map.Remove(key);
        }

        public static void UpdateLockDesignation(Thing t)
        {
            bool flag = false;
            Building_Door door = t as Building_Door;
            if (door != null)
            {
                flag = GetData(door).NeedChange;
            }
            Designation designation = t.Map.designationManager.DesignationOn(t, DesDef);
            if (flag && designation == null)
            {
                t.Map.designationManager.AddDesignation(new Designation(t, DesDef));
                door.Map.reachability.ClearCache();
            }
            else if (!flag && designation != null)
            {
                designation.Delete();
            }
        }

        public static bool IsVisible(this LockState state, string propertyName)
        {
            switch (propertyName)
            {
                case nameof(LockState.locked):
                    return true;
                case nameof(LockState.mode):
                    return state.locked && !state.Private;
                case nameof(LockState.petDoor):
                    return state.locked;
                case nameof(LockState.pensDoor):
                    return state.locked;
                case nameof(LockState.owners):
                    return state.locked;
                default:
                    return true;
            }
        }
    }
}
