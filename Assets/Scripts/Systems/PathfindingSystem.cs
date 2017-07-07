using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Systems
{
    class PathfindingSystem : IFrameEntitySystem, IPausableSystem
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
                var navAgent = entity.GameObject.GetComponent<NavMeshAgent>();

                navAgent.enabled = pathfindingState.IsActive;

                var positionGoal = pathfindingState.GetTargetPosition();
                var rotationGoal = pathfindingState.GetTargetRotation();
                var paused = pathfindingState.GetPaused();
                var stoppingDistance = pathfindingState.GetStoppingDistance();

                if (positionGoal.HasValue && navAgent.isActiveAndEnabled)
                {
                    navAgent.updateRotation = true;
                    navAgent.destination = positionGoal.Value;
                    navAgent.stoppingDistance = stoppingDistance;
                }
                else if (rotationGoal.HasValue)
                {
                    navAgent.updateRotation = false;
                }

                if (paused)
                {
                    navAgent.isStopped = true;
                    navAgent.velocity = Vector3.zero;
                }
                else if(navAgent.isActiveAndEnabled)
                {
                    navAgent.isStopped = false;
                }
            }
        }

        public void Pause(List<Entity> matchingEntities)
        {
            foreach (var entity in matchingEntities)
            {
                var navAgent = entity.GameObject.GetComponent<NavMeshAgent>();
                if (navAgent.isActiveAndEnabled)
                {
                    navAgent.isStopped = true;
                }
            }
        }

        public void Resume(List<Entity> matchingEntities)
        {
            foreach (var entity in matchingEntities)
            {
                var navAgent = entity.GameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (navAgent.isActiveAndEnabled)
                {
                    navAgent.isStopped = false;
                }
            }
        }
    }
}
