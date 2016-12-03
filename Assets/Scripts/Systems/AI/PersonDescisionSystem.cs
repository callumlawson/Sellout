using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Systems.AI.AIActions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Systems.AI
{
    class PersonDescisionSystem : ITickEntitySystem
    {
        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(GoalState), typeof(InventoryState) };
        }

        public void Tick(List<Entity> matchingEntities)
        {
            foreach (var entity in matchingEntities)
            {
                if (ActionManagerSystem.Instance.IsEntityIdle(entity))
                {
                    if (entity.GetState<InventoryState>().child != null)
                    {
                        ActionManagerSystem.Instance.QueueActionForEntity(entity, new PayForAction());
                    }
                    else
                    {
                        var xyPos = Random.insideUnitCircle * 15;
                        ActionManagerSystem.Instance.QueueActionForEntity(entity, new GoToPositionAction(new Vector3(xyPos.x, 0.0f, xyPos.y)));
                    }
                }
            }
        }
    }
}
