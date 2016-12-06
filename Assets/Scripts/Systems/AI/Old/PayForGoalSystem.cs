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
            return new List<Type> { typeof(GoalState), typeof(PathfindingState) };
        }

        public void Tick(List<Entity> matchingEntities)
        {
            foreach (var entity in matchingEntities)
            {
                var goalState = entity.GetState<GoalState>();
                if (goalState.PreviousGoal == Goal.PayFor)
                {
                    var payPoints = entitySystem.GetEntitiesWithState<GoalSatisfierState>()
                                    .Where(satisfierEntity => satisfierEntity.GetState<GoalSatisfierState>().SatisfiedGoals.Contains(Goal.PayFor));
                    foreach (var payPoint in payPoints)
                    {
                        if (Equals(payPoint.GetState<UserState>().Reserver, entity))
                        {
                            payPoint.GetState<UserState>().Reserver = null;
                        }
                    }
                }
                else if (goalState.CurrentGoal == Goal.PayFor && goalState.CurrentGoalStatus == GoalStatus.Start)
                {
                    var goalSatisfiers = entitySystem.GetEntitiesWithState<GoalSatisfierState>();
                    var payPoint = goalSatisfiers
                        .Where(satisfierEntity => satisfierEntity.GetState<UserState>().IsFree())
                        .FirstOrDefault(satisfierEntity => satisfierEntity.GetState<GoalSatisfierState>().SatisfiedGoals.Contains(Goal.PayFor));
                    if (payPoint != null)
                    {
                        entity.GetState<GoalState>().UpdateGoalStatus(GoalStatus.Ongoing);
                        payPoint.GetState<UserState>().Reserver = entity;
                        entity.GetState<PathfindingState>().TargetPosition = payPoint.GetState<PositionState>().Position;
                    }
                }
            }
        }
    }
}
