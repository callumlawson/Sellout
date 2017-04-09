using System.Collections.Generic;
using System.Linq;
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
                        ActionManagerSystem.Instance.QueueAction(entity,
                            GoToPaypointOrderDrinkAndSitDown(entity, drinkRecipe));
                    }
                    else
                    {
                        ActionManagerSystem.Instance.QueueAction(entity, Wander());
                    }
                }
            }
        }

        public static ActionSequence TalkToPlayer(Conversation conversation)
        {
            var player = StaticStates.Get<PlayerState>().Player;
            var talking = new ActionSequence("TalkToPlayer", isCancellable: false);
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
            return wander;
        }

        public static ActionSequence WalkToWaypoint()
        {
            var walk = new ActionSequence("Walk to Random Waypoint");
            var waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
            var waypointPositions = waypoints.Select(go => go.transform.position).ToList();
            var targetPosition = waypointPositions[Random.Range(0, waypointPositions.Count)];
            walk.Add(new PauseAction(Random.Range(0, 10)));
            walk.Add(new GoToPositionAction(targetPosition));
            return walk;
        }

        public static ActionSequence LeaveBar()
        {
            var leave = new ActionSequence("Leaving");
            leave.Add(new GoToPositionAction(Constants.OffstagePostion));
            return leave;
        }

        public static ConditionalActionSequence QueueForDrinkOrder(Entity entity, int findWaypointTimeout = 0, int getToWaypointTimeout = 0)
        {
            var queueForDrink = new ConditionalActionSequence("QueueForDrink");

            queueForDrink.Add(
            new OnFailureDecorator(
               new GetWaypointAction(Goal.PayFor, true, true, findWaypointTimeout),
               () =>
               {
                   ActionManagerSystem.Instance.QueueAction(entity, new ReleaseWaypointAction());
                   ActionManagerSystem.Instance.QueueAction(entity, new UpdateMoodAction(Mood.Angry));
               }
            ));
            queueForDrink.Add(new GoToWaypointAction());
            queueForDrink.Add(
            new OnFailureDecorator(
                new WaitForWaypointWithUserAction(Goal.RingUp, StaticStates.Get<PlayerState>().Player, getToWaypointTimeout), //Does not reserve wayponit.
                () =>
                {
                    ActionManagerSystem.Instance.QueueAction(entity, new ReleaseWaypointAction());
                    ActionManagerSystem.Instance.QueueAction(entity, new UpdateMoodAction(Mood.Angry));
                    ActionManagerSystem.Instance.QueueAction(entity, Wander());
                })
            );
            return queueForDrink;
        }

        public static ConditionalActionSequence OrderDrinkFromPayPoint(Entity entity, DrinkRecipe drinkRecipe, int timeout = 0)
        {
            var orderDrink = new ConditionalActionSequence("OrderDrinkFromPaypoint");
            orderDrink.Add(new ConversationAction(new Dialogues.OrderDrinkConverstation(drinkRecipe.DrinkName)));
            orderDrink.Add(new StartDrinkOrderAction(drinkRecipe));
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
                   ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new EndDrinkOrderAction());
                   ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new ReleaseWaypointAction());
                   ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new ConversationAction(Dialogues.WrongDrinkDialogue));
                   ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new UpdateMoodAction(Mood.Angry));
                   ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new DestoryEntityInInventoryAction());
               })
            );
            waitForDrink.Add(new EndDrinkOrderAction());
            waitForDrink.Add(new ReleaseWaypointAction());
            waitForDrink.Add(new UpdateMoodAction(Mood.Happy));
            waitForDrink.Add(new ModifyMoneyAction(Constants.DrinkSucsessMoney));
            return waitForDrink;
        }

        public static ConditionalActionSequence GoToPaypointAndOrderDrink(Entity entity, DrinkRecipe drinkRecipe, int orderTimeoutInMins = 20)
        {
            var orderDrink = new ConditionalActionSequence("OrderDrinkIfPossible");
            orderDrink.Add(QueueForDrinkOrder(entity, 10, 20));
            orderDrink.Add(OrderDrinkFromPayPoint(entity, drinkRecipe, orderTimeoutInMins));           
            return orderDrink;
        }

        public static ConditionalActionSequence GoToPaypointOrderDrinkAndSitDown(Entity entity, DrinkRecipe drinkRecipe, int orderTimeoutInMins = 20)
        {
            var orderingAndDrinking = new ConditionalActionSequence("OrderingAndDrinking");
            
            var orderDrink = GoToPaypointAndOrderDrink(entity, drinkRecipe, orderTimeoutInMins);
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
