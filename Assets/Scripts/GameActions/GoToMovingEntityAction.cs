using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.GameActions
{
    class GoToMovingEntityAction : GameAction, ICancellableAction
    {
        private const float PositionTolerance = 0.1f;
        private PathfindingState pathfindingState;
        private Entity targetWaypoint;
        private float stoppingDistance = 2.0f;

        public GoToMovingEntityAction()
        {

        }

        public override void OnStart(Entity entity)
        {
            pathfindingState = entity.GetState<PathfindingState>();
            targetWaypoint = entity.GetState<ActionBlackboardState>().TargetEntity;
            if (targetWaypoint != null)
            {
                pathfindingState.SetNewTarget(targetWaypoint.GetState<PositionState>().Position);
                pathfindingState.SetStoppingDistance(stoppingDistance);
            }
            else
            {
                Debug.LogWarning("GotToMovingWaypointAction failed as there was no waypoint. This shouldn't happen");
                ActionStatus = ActionStatus.Failed;
            }
        }

        public override void OnFrame(Entity entity)
        {
            pathfindingState.SetNewTarget(targetWaypoint.GetState<PositionState>().Position);

            if (Vector3.Distance(entity.GetState<PositionState>().Position, pathfindingState.GetTargetPosition().GetValueOrDefault()) < pathfindingState.GetStoppingDistance() + PositionTolerance)
            {
                pathfindingState = entity.GetState<PathfindingState>();
                pathfindingState.ClearTarget();
                ActionStatus = ActionStatus.Succeeded;
            }
            //TODO: Add timeout => Failure.
        }

        public override void Pause()
        {
            pathfindingState.SetPaused(true);
        }

        public override void Unpause()
        {
            pathfindingState.SetPaused(false);
        }

        public void Cancel()
        {
            pathfindingState.ClearTarget();
        }

        public bool IsCancellable()
        {
            return true;
        }
    }
}
