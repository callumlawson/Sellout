using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.GameActions
{
    class GoToPositionAction : GameAction, ICancellableAction
    {
        private const float PositionTolerance = 2.0f;
        private readonly Vector3 targetPosition;
        private PathfindingState pathfindingState;

        public GoToPositionAction(Vector3 position)
        {
            targetPosition = position;
        }

        public override void OnStart(Entity entity)
        {
            pathfindingState = entity.GetState<PathfindingState>();
            pathfindingState.SetNewTarget(targetPosition);
            pathfindingState.SetStoppingDistance(0f);
        }

        public override void OnFrame(Entity entity)
        {
            if (Vector3.Distance(entity.GetState<PositionState>().Position, targetPosition) < PositionTolerance)
            {
                pathfindingState = entity.GetState<PathfindingState>();
                pathfindingState.ClearTarget();
                ActionStatus = ActionStatus.Succeeded;
            }
            //TODO: Add timeout.
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
