using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.GameActions
{
    class GoToMovingWaypointAction : GameAction
    {
        private const float PositionTolerance = 2.0f;
        private PathfindingState pathfindingState;
        private Entity targetWaypoint;

        public override void OnStart(Entity entity)
        {
            pathfindingState = entity.GetState<PathfindingState>();
            targetWaypoint = entity.GetState<ActionBlackboardState>().TargetEntity;
            if (targetWaypoint != null)
            {
                pathfindingState.TargetPosition = targetWaypoint.GetState<PositionState>().Position;
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
                ActionStatus = ActionStatus.Succeeded;
            }
            //TODO: Add timeout => Failure.
        }
    }
}
