using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.Systems.AI.AIActions
{
    class GoToPositionAction : GameAction
    {
        private const float PositionTolerance = 2.0f;
        private readonly Vector3 targetPosition;

        public GoToPositionAction(Vector3 position)
        {
            targetPosition = position;
        }

        public override void OnStart(Entity entity)
        {
            var pathfinding = entity.GetState<PathfindingState>();
            pathfinding.TargetPosition = targetPosition;
        }

        public override void OnFrame(Entity entity)
        {
            IsComplete = Vector3.Distance(entity.GetState<PositionState>().Position, targetPosition) < PositionTolerance;
        }
    }
}
