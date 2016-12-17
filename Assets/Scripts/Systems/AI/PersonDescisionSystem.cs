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
                    if (Random.value > 0.8f)
                    {
                        var drinkRecipe = DrinkRecipes.GetRandomDrinkRecipe();
                        ActionManagerSystem.Instance.QueueActionForEntity(entity, OrderDrink(entity, drinkRecipe));
                    }
                    else
                    {
                        ActionManagerSystem.Instance.QueueActionForEntity(entity, Wander());
                    }
                }
            }
        }

        private static ActionSequence OrderDrink(Entity entity, DrinkRecipe drinkRecipe)
        {
            var orderingAndDrinking = new ActionSequence("OrderingAndDrinking");
            var orderDrink = new ConditionalActionSequence("OrderThenDrink");

            orderingAndDrinking.Add(orderDrink);
            orderingAndDrinking.Add(new ReleaseWaypointAction());

            orderDrink.Add(
            new OnFailureDecorator(
               new GetWaypointAction(Goal.PayFor, true, true, 10),
               () =>
               {
                   ActionManagerSystem.Instance.QueueActionForEntity(entity, new UpdateMoodAction(Mood.Angry));
               }
            ));
            orderDrink.Add(new GoToWaypointAction());
            //There is a bug here where the payfor waypoint can be left reserved.
            orderDrink.Add(
            new OnFailureDecorator(
                new WaitForWaypointWithUserAction(Goal.RingUp, StaticStates.Get<PlayerState>().Player, 20), //Does not reserve wayponit.
                () =>
                {
                    ActionManagerSystem.Instance.QueueActionForEntity(entity, new ReleaseWaypointAction());
                    ActionManagerSystem.Instance.QueueActionForEntity(entity, new UpdateMoodAction(Mood.Angry));
                    ActionManagerSystem.Instance.QueueActionForEntity(entity, Wander());
                })
            );
            orderDrink.Add(new ConversationAction(new Dialogues.OrderDrinkConverstation(drinkRecipe.DrinkName)));
            orderDrink.Add(new OnFailureDecorator(
               new DrinkIsInInventoryAction(new DrinkState(drinkRecipe.Contents), 20), //TODO: Need to account for the "No drink" case here.
               () =>
               {
                   ActionManagerSystem.Instance.QueueActionForEntity(entity, new ReleaseWaypointAction());
                   ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new ConversationAction(Dialogues.WrongDrinkDialogue));
                   ActionManagerSystem.Instance.QueueActionForEntity(entity, new UpdateMoodAction(Mood.Angry));
                   ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new DestoryEntityInInventoryAction());
               })
            );
            orderDrink.Add(new ReleaseWaypointAction());
            orderDrink.Add(new UpdateMoodAction(Mood.Happy));
            orderDrink.Add(new GetWaypointAction(Goal.Sit, reserve: true, closest: true));
            orderDrink.Add(new GoToWaypointAction());
            orderDrink.Add(new PauseAction(15.0f));
            orderDrink.Add(new DrinkItemInInventory());
            orderDrink.Add(new ReleaseWaypointAction());
            orderDrink.Add(new GetWaypointAction(Goal.Storage, reserve: false, closest: true));
            orderDrink.Add(new PutDownInventoryItemAtWaypoint());

            return orderingAndDrinking;
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
