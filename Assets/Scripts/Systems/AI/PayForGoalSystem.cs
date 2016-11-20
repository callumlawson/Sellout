using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Util;

namespace Assets.Scripts.Systems.AI
{
    class PayForGoalSystem : ITickEntitySystem, IEntityManager
    {
        private EntityStateSystem entitySystem;

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(CurrentGoalState), typeof(PathfindingState) };
        }

        public void Tick(List<Entity> matchingEntities)
        {
            foreach (var entity in matchingEntities)
            {
                if (entity.GetState<CurrentGoalState>().CurrentGoal == Goal.PayFor)
                {
                    var goalSatisfiers = entitySystem.GetEntitiesWithState<GoalSatisfierState>();
                    var payPoint = goalSatisfiers.FirstOrDefault(satisfierEntity => satisfierEntity.GetState<GoalSatisfierState>().SatisfiedGoals.Contains(Goal.PayFor));
                    if (payPoint != null)
                    {
                        entity.GetState<PathfindingState>().TargetPosition = payPoint.GetState<PositionState>().Position;
                    }
                }
            }
        }

    }
}
