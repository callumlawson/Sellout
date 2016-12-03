using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.GameActions
{
    class GoToWaypointAction : GameAction
    {
        private const float PositionTolerance = 2.0f;
        private PathfindingState pathfindingState;

        public override void OnStart(Entity entity)
        {
            pathfindingState = entity.GetState<PathfindingState>();
            var targetWaypoint = entity.GetState<ActionBlackboardState>().TargetEntity;
            if (targetWaypoint != null)
            {
                pathfindingState.TargetPosition = targetWaypoint.GetState<PositionState>().Position;
            }
            else
            {
                Debug.LogWarning("GotToWaypointAction failed as there was no waypoint. This shouldn't happen");
                ActionStatus = ActionStatus.Failed;
            }
        }

        public override void OnFrame(Entity entity)
        {
            if (Vector3.Distance(entity.GetState<PositionState>().Position, pathfindingState.TargetPosition.GetValueOrDefault()) < PositionTolerance)
            {
                ActionStatus = ActionStatus.Succeeded;
            }
            //TODO: Add timeout => Failure.
        }
    }
}
