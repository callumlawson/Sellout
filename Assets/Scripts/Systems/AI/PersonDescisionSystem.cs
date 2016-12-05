﻿using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Systems.AI
{
    class PersonDescisionSystem : ITickEntitySystem
    {
        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(PersonState) };
        }

        public void Tick(List<Entity> matchingEntities)
        {
            foreach (var entity in matchingEntities)
            {
                if (ActionManagerSystem.Instance.IsEntityIdle(entity))
                {
                    if (entity.GetState<InventoryState>().child != null)
                    {
                        ActionManagerSystem.Instance.QueueActionForEntity(entity, Drink());
                    }
                    else
                    {
                        var xyPos = Random.insideUnitCircle * 6;
                        ActionManagerSystem.Instance.QueueActionForEntity(entity, Wander(new Vector3(xyPos.x, 0.0f, xyPos.y)));
                    }
                }
            }
        }

        private static ActionSequence Drink()
        {
            var drink = new ActionSequence();
            drink.Add(new GetWaypointAction(Goal.PayFor));
            drink.Add(new GoToWaypointAction());
            drink.Add(new PauseAction(10.0f));
            drink.Add(new GetWaypointAction(Goal.Sit));
            drink.Add(new GoToWaypointAction());
            drink.Add(new PauseAction(30.0f));
            drink.Add(new DestoryEntityInInventoryAction());
            return drink;
        }

        private static ActionSequence Wander(Vector3 randomPosition)
        {
            var wander = new ActionSequence();
            wander.Add(new GoToPositionAction(new Vector3(randomPosition.x, 0.0f, randomPosition.z)));
            wander.Add(new PauseAction(4.0f));
            return wander;
        }
    }
}
