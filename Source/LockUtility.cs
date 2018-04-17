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
            if (canOpenAnyDoor || noFaction)
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

            if (respectedState.Private && respectedState.petDoor && p.RaceProps.Animal && p.RaceProps.baseBodySize <= 0.85 && p.Faction == door.Faction)
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

    }
}
