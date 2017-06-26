using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Util
{
    public static class Locations
    {
        public static Vector3 OutsideDoorLocation()
        {
            return GameObject.FindGameObjectWithTag("SpawnPoint").transform.position + (Vector3) Random.insideUnitCircle;
        }

        public static Vector3 RandomHallwayEndLocation()
        {
            var waypointPositions = GameObject.FindGameObjectsWithTag("Waypoint").Select(go => go.transform.position).ToList();
            return waypointPositions[Random.Range(0, waypointPositions.Count)];
        }

        public static Transform BehindBarLocation()
        {
            return GameObject.FindGameObjectWithTag("BarSpawnPoint").transform;
        }

        public static Transform RandomSeatLocation()
        {
            return WaypointSystem.Instance.GetFreeWaypointThatSatisfyGoal(Goal.Sit).GameObject.transform;
        }

        public static Transform SitDownPoint1()
        {
            return GameObject.FindGameObjectWithTag("SitDownPoint1").transform;
        }

        public static Transform SitDownPoint2()
        {
            return GameObject.FindGameObjectWithTag("SitDownPoint2").transform;
        }

        public static Transform SitDownPoint3()
        {
            return GameObject.FindGameObjectWithTag("SitDownPoint3").transform;
        }

        public static Transform CenterOfBar()
        {
            return GameObject.FindGameObjectWithTag("CenterOfBarPoint").transform;
        }

        public static void ResetPeopleToSpawnPoints(List<Entity> people)
        {
            foreach (var person in people)
            {
                if (person.HasState<NameState>() && person.GetState<NameState>().Name == "You")
                {
                    person.GetState<PositionState>().Teleport(BehindBarLocation().position);
                    person.GetState<RotationState>().Teleport(BehindBarLocation().rotation);
                }
                else if (person.HasState<NameState>() && person.GetState<NameState>().Name == "Expendable")
                {
                    person.GetState<PositionState>().Teleport(RandomHallwayEndLocation());
                }
                else
                {
                    person.GetState<PositionState>().Teleport(OutsideDoorLocation());
                }
            }
        }
    }
}
