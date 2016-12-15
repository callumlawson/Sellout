using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.GameActions.Decorators;
using Assets.Scripts.GameActions.Inventory;
using Assets.Scripts.GameActions.Waypoints;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Dialogue;
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
                    if (entity.GetState<NameState>().Name == "Tolstoy") //Short term debug!
                    {
                        ActionManagerSystem.Instance.QueueActionForEntity(entity, OrderDrink(entity));
                    }
                    else
                    {
                        ActionManagerSystem.Instance.QueueActionForEntity(entity, Wander());
                    }
                }
            }
        }

        private static ConditionalActionSequence OrderDrink(Entity entity)
        {
            var orderDrink = new ConditionalActionSequence("Order Drink");
            var drinkDrink = new ActionSequence("Drink Drink");

            orderDrink.Add(new GetWaypointAction(Goal.PayFor));
            orderDrink.Add(new GoToWaypointAction());
            orderDrink.Add(new PauseAction(3.0f));
            orderDrink.Add(
            new OnFailureDecorator(
                new GetWaypointWithUserAction(Goal.RingUp, StaticStates.Get<PlayerState>().Player, 20),
                () =>
                {
                    ActionManagerSystem.Instance.QueueActionForEntity(entity, new UpdateMoodAction(Mood.Angry));
                    ActionManagerSystem.Instance.QueueActionForEntity(entity, Wander());
                })
            );
            orderDrink.Add(new ConversationAction(Dialogues.OrderDrinkDialogue));

            orderDrink.Add(drinkDrink);
            drinkDrink.Add(new OnFailureDecorator(
               new DrinkIsInInventoryAction(new DrinkState(DrinkUI.screwdriverIngredients), 20), //TODO: Need to account for the "No drink" case here.
               () => ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new ConversationAction(Dialogues.WrongDrinkDialogue)))
            );
            drinkDrink.Add(new GetWaypointAction(Goal.Sit, reserve: true, closest: true));
            drinkDrink.Add(new GoToWaypointAction());
            drinkDrink.Add(new PauseAction(15.0f));
            drinkDrink.Add(new DrinkItemInInventory());
            drinkDrink.Add(new ReleaseWaypointAction());
            drinkDrink.Add(new GetWaypointAction(Goal.Storage, reserve: false, closest: true));
            drinkDrink.Add(new PutDownInventoryItemAtWaypoint());
            return orderDrink;
        }

        private static ActionSequence Wander()
        {
            var wander = new ActionSequence("Wander Around");
            var xyPos = Random.insideUnitCircle * 6;
            wander.Add(new GoToPositionAction(new Vector3(xyPos.x, 0.0f, xyPos.y)));
            wander.Add(new PauseAction(4.0f));
            if (Random.value > 0.80)
            {
                wander.Add(new GetWaypointAction(Goal.Sit, reserve: true));
                wander.Add(new GoToWaypointAction());
                wander.Add(new PauseAction(40.0f));
                wander.Add(new ReleaseWaypointAction());
            }
            return wander;
        }
    }
}
