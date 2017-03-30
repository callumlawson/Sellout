using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.GameActions.Decorators;
using Assets.Scripts.GameActions.Dialogue;
using Assets.Scripts.GameActions.Inventory;
using Assets.Scripts.GameActions.Waypoints;
using Assets.Scripts.States;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Dialogue;
using UnityEngine;

namespace Assets.Scripts.GameActions
{
    static class CommonActions
    {
        public static void DrinkOrWanderAroundIfIdle(List<Entity> matchingEntities)
        {
            foreach (var entity in matchingEntities)
            {
                if (ActionManagerSystem.Instance.IsEntityIdle(entity))
                {
                    if (Random.value > 0.8f)
                    {
                        var drinkRecipe = DrinkRecipes.GetRandomDrinkRecipe();
                        ActionManagerSystem.Instance.QueueActionForEntity(entity,
                            GoToPaypointOrderDrinkAndSitDown(entity, drinkRecipe));
                    }
                    else
                    {
                        ActionManagerSystem.Instance.QueueActionForEntity(entity, Wander());
                    }
                }
            }
        }

        public static ActionSequence TalkToPlayer(Conversation conversation)
        {
            var player = StaticStates.Get<PlayerState>().Player;
            var talking = new ActionSequence("TalkToPlayer");
            talking.Add(new SetTargetEntityAction(player));
            talking.Add(new GoToMovingEntityAction());
            talking.Add(new ConversationAction(conversation));
            return talking;
        }
 
        public static ActionSequence Wander()
        {
            var wander = new ActionSequence("Wander Around");
            var xyPos = Random.insideUnitCircle * 6;
            wander.Add(new GoToPositionAction(new Vector3(xyPos.x, 0.0f, xyPos.y)));
            wander.Add(new PauseAction(4.0f));
            if (Random.value > 0.80)
            {
                wander.Add(new GetWaypointAction(Goal.Sit, reserve: true));
                wander.Add(new GoToWaypointAction());
                wander.Add(new PauseAction(20.0f));
                wander.Add(new ReleaseWaypointAction());
            }
            return wander;
        }

        public static ConditionalActionSequence QueueForDrinkOrder(Entity entity, int findWaypointTimeout = 0, int getToWaypointTimeout = 0)
        {
            var queueForDrink = new ConditionalActionSequence("QueueForDrink");

            queueForDrink.Add(
            new OnFailureDecorator(
               new GetWaypointAction(Goal.PayFor, true, true, findWaypointTimeout),
               () =>
               {
                   ActionManagerSystem.Instance.QueueActionForEntity(entity, new ReleaseWaypointAction());
                   ActionManagerSystem.Instance.QueueActionForEntity(entity, new UpdateMoodAction(Mood.Angry));
               }
            ));
            queueForDrink.Add(new GoToWaypointAction());
            queueForDrink.Add(
            new OnFailureDecorator(
                new WaitForWaypointWithUserAction(Goal.RingUp, StaticStates.Get<PlayerState>().Player, getToWaypointTimeout), //Does not reserve wayponit.
                () =>
                {
                    ActionManagerSystem.Instance.QueueActionForEntity(entity, new ReleaseWaypointAction());
                    ActionManagerSystem.Instance.QueueActionForEntity(entity, new UpdateMoodAction(Mood.Angry));
                    ActionManagerSystem.Instance.QueueActionForEntity(entity, Wander());
                })
            );
            return queueForDrink;
        }

        public static ConditionalActionSequence OrderDrinkFromPayPoint(Entity entity, DrinkRecipe drinkRecipe, int timeout = 0)
        {
            var orderDrink = new ConditionalActionSequence("OrderDrinkFromPaypoint");
            orderDrink.Add(new ConversationAction(new Dialogues.OrderDrinkConverstation(drinkRecipe.DrinkName)));
            orderDrink.Add(WaitForDrink(entity, drinkRecipe, timeout));
            return orderDrink;
        }

        public static ConditionalActionSequence WaitForDrink(Entity entity, DrinkRecipe drinkRecipe, int timeoutInGameMins)
        {
            var waitForDrink = new ConditionalActionSequence("WaitForDrink");
            waitForDrink.Add(new OnFailureDecorator(
               new DrinkIsInInventoryAction(new DrinkState(drinkRecipe.Contents), timeoutInGameMins), //TODO: Need to account for the "No drink" case here.
               () =>
               {
                   ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new ReleaseWaypointAction());
                   ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new ConversationAction(Dialogues.WrongDrinkDialogue));
                   ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new UpdateMoodAction(Mood.Angry));
                   ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new DestoryEntityInInventoryAction());
               })
            );
            waitForDrink.Add(new ReleaseWaypointAction());
            waitForDrink.Add(new UpdateMoodAction(Mood.Happy));
            waitForDrink.Add(new ModifyMoneyAction(Constants.DrinkSucsessMoney));
            return waitForDrink;
        }

        public static ConditionalActionSequence GoToPaypointAndOrderDrink(Entity entity, DrinkRecipe drinkRecipe)
        {
            var orderDrink = new ConditionalActionSequence("OrderDrinkIfPossible");
            orderDrink.Add(QueueForDrinkOrder(entity, 10, 20));
            orderDrink.Add(OrderDrinkFromPayPoint(entity, drinkRecipe, 20));           
            return orderDrink;
        }

        public static ConditionalActionSequence GoToPaypointOrderDrinkAndSitDown(Entity entity, DrinkRecipe drinkRecipe)
        {
            var orderingAndDrinking = new ConditionalActionSequence("OrderingAndDrinking");
            
            var orderDrink = GoToPaypointAndOrderDrink(entity, drinkRecipe);
            orderingAndDrinking.Add(orderDrink);

            var sitDown = new ActionSequence("Sit down");
            orderingAndDrinking.Add(sitDown);

            sitDown.Add(new GetWaypointAction(Goal.Sit, reserve: true, closest: true)); //This assumes more seats than NPCs!
            sitDown.Add(new GoToWaypointAction());
            sitDown.Add(new PauseAction(15.0f));
            sitDown.Add(new DrinkItemInInventory());
            sitDown.Add(new ReleaseWaypointAction());
            sitDown.Add(new GetWaypointAction(Goal.Storage, reserve: false, closest: true));
            sitDown.Add(new PutDownInventoryItemAtWaypoint());

            return orderingAndDrinking;
        }
    }
}
