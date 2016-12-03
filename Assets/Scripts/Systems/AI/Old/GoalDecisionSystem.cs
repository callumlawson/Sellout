using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Systems.AI
{
    class GoalDecisionSystem : ITickEntitySystem
    {
        public List<Type> RequiredStates()
        {
            return new List<Type> {typeof(GoalState), typeof(PersonState)};
        }

        public void Tick(List<Entity> matchingEntities)
        {
            foreach (var entity in matchingEntities)
            {
                if (entity.GetState<InventoryState>().child != null)
                {
                    entity.GetState<GoalState>().UpdateGoal(Goal.PayFor);
                    entity.GetState<GoalState>().UpdateGoalStatus(GoalStatus.Start);
                }
                else if (Random.value > 0.9)
                {
                    var goals = new List<Goal> {Goal.Wander, Goal.Sit};
                    var randomGoal = goals[Random.Range(0, goals.Count)];
                    if (randomGoal != entity.GetState<GoalState>().CurrentGoal)
                    {
                        entity.GetState<GoalState>().UpdateGoal(randomGoal);
                        entity.GetState<GoalState>().UpdateGoalStatus(GoalStatus.Start);
                    }
                }
            }
        }
    }
}
