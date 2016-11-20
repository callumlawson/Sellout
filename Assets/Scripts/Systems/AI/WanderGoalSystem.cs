using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Systems.AI
{
    class WanderGoalSystem : ITickEntitySystem
    {
        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(GoalState) };
        }

        public void Tick(List<Entity> matchingEntities)
        {
            foreach (var entity in matchingEntities)
            {
                if (entity.GetState<GoalState>().CurrentGoal == Goal.Wander)
                {
                    if (Random.value > 0.6)
                    {
                        var xyPos = Random.insideUnitCircle * 15;
                        entity.GetState<PathfindingState>().TargetPosition = new Vector3(xyPos.x, 0.0f, xyPos.y);
                    }
                }
            }
        }
    }
}