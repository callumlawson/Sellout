using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    class PathfindingSystem : IFrameEntitySystem
    {
        public List<Type> RequiredStates()
        {
            return new List<Type> {typeof(PathfindingState)};
        }

        public void OnFrame(List<Entity> matchingEntities)
        {
            foreach (var entity in matchingEntities)
            {
                var pathfindingState = entity.GetState<PathfindingState>();
                var goal = pathfindingState.GetTargetPosition();
                var paused = pathfindingState.GetPaused();
                var stoppingDistance = pathfindingState.GetStoppingDistance();

                var navAgent = entity.GameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (goal.HasValue)
                {
                    navAgent.destination = goal.Value;
                    navAgent.stoppingDistance = stoppingDistance;
                }

                if (paused)
                {
                    navAgent.isStopped = true;
                    navAgent.velocity = Vector3.zero;
                }
                else
                {
                    navAgent.isStopped = false;
                }
            }
        }
    }
}
