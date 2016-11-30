using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    class PathfindingSystem : ITickEntitySystem
    {
        public List<Type> RequiredStates()
        {
            return new List<Type> {typeof(PathfindingState)};
        }

        public void Tick(List<Entity> matchingEntities)
        {
            foreach (var entity in matchingEntities)
            {
                var goal = entity.GetState<PathfindingState>().TargetPosition;

                var navAgent = entity.GameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (goal.HasValue)
                {
                    navAgent.destination = goal.Value;
                }
            }
        }
    }
}
