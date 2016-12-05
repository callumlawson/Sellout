using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.GameActions
{
    class GoToMovingEntityAction : GameAction
    {
        private const float PositionTolerance = 2.0f;
        private PathfindingState pathfindingState;
        private Entity targetWaypoint;
        private float stoppingDistance;

        public GoToMovingEntityAction(float stoppingDistance)
        {
            this.stoppingDistance = stoppingDistance;
        }

        public override void OnStart(Entity entity)
        {
            pathfindingState = entity.GetState<PathfindingState>();
            targetWaypoint = entity.GetState<ActionBlackboardState>().TargetWaypoint;
            if (targetWaypoint != null)
            {
                pathfindingState.TargetPosition = targetWaypoint.GetState<PositionState>().Position;
                pathfindingState.StoppingDistance = stoppingDistance;
            }
            else
            {
                Debug.LogWarning("GotToMovingWaypointAction failed as there was no waypoint. This shouldn't happen");
                ActionStatus = ActionStatus.Failed;
            }
        }

        public override void OnFrame(Entity entity)
        {
            pathfindingState.TargetPosition = targetWaypoint.GetState<PositionState>().Position;

            if (Vector3.Distance(entity.GetState<PositionState>().Position, pathfindingState.TargetPosition.GetValueOrDefault()) < PositionTolerance)
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
    }
}
