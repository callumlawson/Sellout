using System;
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
                        ActionManagerSystem.Instance.QueueActionForEntity(entity, Wander());
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
            //EntityIsUsingWaypoint(Goal.RingUp, PlayerEntity)
            //StartConversation()
            //EntityIsInInventory()
            drink.Add(new GetAndUseWaypointAction(Goal.Sit));
            drink.Add(new GoToWaypointAction());
            drink.Add(new PauseAction(30.0f));
            drink.Add(new DestoryEntityInInventoryAction());
            drink.Add(new StopUsingWaypointAction());//Remove?
            return drink;
        }

        private static ActionSequence Wander()
        {
            var wander = new ActionSequence();
            var xyPos = Random.insideUnitCircle * 6;
            wander.Add(new GoToPositionAction(new Vector3(xyPos.x, 0.0f, xyPos.y)));
            wander.Add(new PauseAction(4.0f));
            if (Random.value > 0.80)
            {
                wander.Add(new GetAndUseWaypointAction(Goal.Sit));
                wander.Add(new GoToWaypointAction());
                wander.Add(new PauseAction(40.0f));
                wander.Add(new StopUsingWaypointAction());
            }
            return wander;
        }
    }
}
