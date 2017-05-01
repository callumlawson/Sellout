using System.Collections.Generic;
using System.Linq;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.States;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Util
{
    public static class SpawnPoints
    {
        public static Vector3 BarVisitorSpawnPoint()
        {
            return GameObject.FindGameObjectWithTag("SpawnPoint").transform.position;
        }

        public static Vector3 RandomHallwaySpawnPoint()
        {
            var waypointPositions = GameObject.FindGameObjectsWithTag("Waypoint").Select(go => go.transform.position).ToList();
            return waypointPositions[Random.Range(0, waypointPositions.Count)];
        }

        private static Vector3 BehindBarSpawnPoint()
        {
            return GameObject.FindGameObjectWithTag("BarSpawnPoint").transform.position;
        }

        public static void ResetPeopleToSpawnPoints(List<Entity> people)
        {
            foreach (var person in people)
            {
                if (person.HasState<NameState>() && person.GetState<NameState>().Name == "You")
                {
                    person.GetState<PositionState>().Teleport(BehindBarSpawnPoint());
                }
                else if (person.HasState<NameState>() && person.GetState<NameState>().Name == "Expendable")
                {
                    person.GetState<PositionState>().Teleport(RandomHallwaySpawnPoint());
                }
                else
                {
                    person.GetState<PositionState>().Teleport(BarVisitorSpawnPoint());
                }
            }
        }
    }
}
