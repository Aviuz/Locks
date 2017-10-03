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
                if(designationDef == null)
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

            if (GetData(door).Locked == false && p.RaceProps != null && p.RaceProps.intelligence >= Intelligence.Humanlike)
                return true;

            if (p.Faction == null || p.Faction.HostileTo(door.Faction))
                return false;

            if (GetData(door).Private && !GetData(door).Owners.Contains(p))
                return false;

            if (p.Faction == door.Faction)
                return true;

            bool guestCondition = !p.IsPrisoner || p.HostFaction != door.Faction;
            if (GetData(door).Mode == LockMode.Allies && guestCondition)
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
                if (GetData(door).Locked != GetData(door).WantLocked)
                    flag = true;
            }
            Designation designation = t.Map.designationManager.DesignationOn(t, DesDef);
            if (flag && designation == null)
            {
                t.Map.designationManager.AddDesignation(new Designation(t, DesDef));
            }
            else if (!flag && designation != null)
            {
                designation.Delete();
            }
        }

    }
}
