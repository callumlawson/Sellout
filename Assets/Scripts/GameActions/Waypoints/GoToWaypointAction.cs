using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.GameActions.Waypoints
{
    class GoToWaypointAction : GameAction, ICancellableAction
    {
        private const float PositionTolerance = 0.5f;
        private PathfindingState pathfindingState;
        private Vector3 targetPosition;

        public override void OnStart(Entity entity)
        {
            pathfindingState = entity.GetState<PathfindingState>();
            var targetWaypoint = entity.GetState<ActionBlackboardState>().TargetEntity;
            if (targetWaypoint != null)
            {
                targetPosition = targetWaypoint.GetState<PositionState>().Position;
                pathfindingState.TargetPosition = targetPosition;
                pathfindingState.StoppingDistance = 0f;
            }
            else
            {
                Debug.LogError("GotToWaypointAction failed as there was no waypoint. This shouldn't happen!");
                ActionStatus = ActionStatus.Failed;
            }
        }

        public override void OnFrame(Entity entity)
        {
            if (Vector3.Distance(entity.GetState<PositionState>().Position, pathfindingState.TargetPosition.GetValueOrDefault()) <= PositionTolerance)
            {
                pathfindingState = entity.GetState<PathfindingState>();
                pathfindingState.TargetPosition = null;
                ActionStatus = ActionStatus.Succeeded;
            }
            //TODO: Add timeout => Failure.
        }        

        public override void Pause()
        {
            pathfindingState.Paused = true;
        }

        public override void Unpause()
        {
            pathfindingState.Paused = false;
        }

        public void Cancel()
        {
            pathfindingState.TargetPosition = null;
        }

        public bool IsCancellable()
        {
            return true;
        }
    }
}
