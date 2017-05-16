using System.Collections.Generic;
using System.Linq;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.States;
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

        private static Vector3 BehindBarLocation()
        {
            return GameObject.FindGameObjectWithTag("BarSpawnPoint").transform.position;
        }

        public static void ResetPeopleToSpawnPoints(List<Entity> people)
        {
            foreach (var person in people)
            {
                if (person.HasState<NameState>() && person.GetState<NameState>().Name == "You")
                {
                    person.GetState<PositionState>().Teleport(BehindBarLocation());
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
