using Locks.CompatibilityPatches;
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

        private static readonly Dictionary<ThingWithComps, LockData> Map = new Dictionary<ThingWithComps, LockData>();

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

        public static bool PawnCanOpen(ThingWithComps door, Pawn p)
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
            {
                return true;
            }

            if (respectedState.pensDoor && p.RaceProps.FenceBlocked && !door.def.building.roamerCanOpen && (!p.roping.IsRopedByPawn || !PawnCanOpen(door, p.roping.RopedByPawn)))
            {
                return false;
            }
            if (p.Faction == null || p.Faction.HostileTo(door.Faction))
            {
                return false;
            }
            if (respectedState.Private && respectedState.petDoor && p.RaceProps != null && p.RaceProps.Animal && p.RaceProps.baseBodySize <= MaxPetSize && p.Faction == door.Faction)
            {
                return true;
            }
            if (!respectedState.allowAnimals && p.RaceProps != null && p.RaceProps.Animal)
            {
                return false;
            }
            if (respectedState.Private && !respectedState.owners.Contains(p))
            {
                return false;
            }

            if (p.Faction == door.Faction && !p.IsPrisoner && !p.IsSlave)
            {
                return true;
            }
            if (respectedState.allowSlave && p.Faction == door.Faction && p.IsSlave)
            {
                return true;
            }
            bool guestCondition = p.GuestStatus == GuestStatus.Guest || !p.IsPrisoner && !p.IsSlave && p.HostFaction != door.Faction;
            if (respectedState.mode == LockMode.Allies && guestCondition)
            {
                return true;
            }

            if (door.Map != null && door.Map.Parent.doorsAlwaysOpenForPlayerPawns && p.Faction == Faction.OfPlayer && !p.IsPrisonerOfColony)
            {
                return true;
            }
            return false;
        }

        public static LockData GetData(ThingWithComps key)
        {
            if (!Map.ContainsKey(key))
                Map[key] = new LockData();
            Map[key].UpdateReference(key);
            return Map[key];
        }

        public static void Remove(ThingWithComps key)
        {
            Map.Remove(key);
        }

        public static void UpdateLockDesignation(Thing t)
        {
            bool flag = false;
            ThingWithComps door = t as ThingWithComps;
            if (door.TryGetComp<CompLock>() != null)
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
                    return true;
                case nameof(LockState.owners):
                    return state.locked;
                case nameof(LockState.allowAnimals):
                    return state.locked;
                case nameof(LockState.allowSlave):
                    return state.locked;
                default:
                    return true;
            }
        }
    }
}
