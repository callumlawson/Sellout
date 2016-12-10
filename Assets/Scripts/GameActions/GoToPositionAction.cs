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
            pathfindingState.TargetPosition = targetPosition;
            pathfindingState.StoppingDistance = 0f;
        }

        public override void OnFrame(Entity entity)
        {
            if (Vector3.Distance(entity.GetState<PositionState>().Position, targetPosition) < PositionTolerance)
            {
                pathfindingState = entity.GetState<PathfindingState>();
                pathfindingState.TargetPosition = null;
                ActionStatus = ActionStatus.Succeeded;
            }
            //TODO: Add timeout.
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
