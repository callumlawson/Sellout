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
        private Vector3 TargetPosition;

        private Entity entity;

        public override void OnStart(Entity entity)
        {
            this.entity = entity;

            pathfindingState = entity.GetState<PathfindingState>();
            var targetWaypoint = entity.GetState<ActionBlackboardState>().TargetEntity;
            if (targetWaypoint != null)
            {
                TargetPosition = targetWaypoint.GetState<PositionState>().Position;
                pathfindingState.TargetPosition = TargetPosition;
                pathfindingState.StoppingDistance = 0f;
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
