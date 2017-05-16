﻿using System.Collections.Generic;
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
using Assets.Scripts.Systems;
using AnimationEvent = Assets.Scripts.Util.AnimationEvent;

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
                        ActionManagerSystem.Instance.QueueAction(entity, GoToPaypointOrderDrinkAndSitDown(entity, drinkRecipe));
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
            var barBounds = BoundsLookup.Instance.GetBarBounds();
            var widthHeight = barBounds.size * 0.8f;
            var offset = new Vector3(widthHeight.x / 2.0f - Random.value * widthHeight.x, 0.0f, widthHeight.z / 2.0f - Random.value * widthHeight.z);
            var target = barBounds.center + offset;
            target.y = 0;
            wander.Add(new GoToPositionAction(target));
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
            leave.Add(new GoToPositionAction(Locations.OutsideDoorLocation()));
            return leave;
        }
        
        public static ActionSequence ShortSitDown(Entity entity)
        {
            var sitDown = new ActionSequence("Short Sit down");
            sitDown.Add(new GetWaypointAction(Goal.Sit, reserve: true, closest: false)); //This assumes more seats than NPCs!
            sitDown.Add(new GoToWaypointAction());
            sitDown.Add(new TriggerAnimationAction(AnimationEvent.SittingStartTrigger));
            sitDown.Add(new PauseAction(6.0f));
            sitDown.Add(new TriggerAnimationAction(AnimationEvent.ChairTalk1Trigger));
            sitDown.Add(new PauseAction(4.0f));
            sitDown.Add(new TriggerAnimationAction(AnimationEvent.ChairTalk1Trigger));
            sitDown.Add(new PauseAction(3.0f));
            sitDown.Add(new TriggerAnimationAction(AnimationEvent.SittingFinishTrigger));
            sitDown.Add(new PauseAction(1.0f)); //Delay for standing up.
            sitDown.Add(new ReleaseWaypointAction());
            sitDown.Add(Wander());
            return sitDown;
        }

        public static ConditionalActionSequence WaitForDrink(Entity entity, DrinkRecipe drinkRecipe, int timeoutInGameMins, bool retry = false)
        {
            var waitForDrink = new ConditionalActionSequence("WaitForDrink");
            waitForDrink.Add(new OnFailureDecorator(
               new DrinkIsInInventoryAction(new DrinkState(drinkRecipe.Contents), timeoutInGameMins), //TODO: Need to account for the "No drink" case here.
               () => {
                   if (entity.GetState<InventoryState>().Child != null)
                   {
                       if (retry) //If retry is true then you are stuck until you don't fail.
                       {
                           ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, WaitForDrink(entity, drinkRecipe, 90, true));
                           ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new ConversationAction(new Dialogues.OrderDrinkRetryConverstation(drinkRecipe.DrinkName)));
                       }
                       else
                       {
                           ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new ConversationAction(Dialogues.WrongDrinkDialogue));
                       }
                       ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new EndDrinkOrderAction());
                       ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new ReleaseWaypointAction());
                       ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new UpdateMoodAction(Mood.Angry));
                       ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new DestoryEntityInInventoryAction());
                   }
                   else
                   {
                       ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new EndDrinkOrderAction());
                       ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new ReleaseWaypointAction());
                       ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(entity, new UpdateMoodAction(Mood.Angry));
                   }
               })
            );
            //Only if not failed
            waitForDrink.Add(new TriggerAnimationAction(AnimationEvent.ItemTakeTrigger));
            waitForDrink.Add(new ModifyMoneyAction(Constants.DrinkSucsessMoney));
            waitForDrink.Add(new PauseAction(0.8f));
            waitForDrink.Add(new EndDrinkOrderAction());
            waitForDrink.Add(new ReleaseWaypointAction());
            waitForDrink.Add(new UpdateMoodAction(Mood.Happy));
            return waitForDrink;
        }

        public static ConditionalActionSequence GoToPaypointAndOrderDrink(Entity entity, DrinkRecipe drinkRecipe, int orderTimeoutInMins = 20)
        {
            var orderDrink = new ConditionalActionSequence("OrderDrinkIfPossible");
            orderDrink.Add(QueueForDrinkOrder(entity, 10, 20));
            orderDrink.Add(OrderDrinkFromPayPoint(entity, drinkRecipe, orderTimeoutInMins));           
            return orderDrink;
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

        public static ConditionalActionSequence OrderDrinkFromPayPoint(Entity entity, DrinkRecipe drinkRecipe, int orderTimeoutInMins = 20)
        {
            var orderDrink = new ConditionalActionSequence("OrderDrinkFromPaypoint");
            orderDrink.Add(new ConversationAction(new Dialogues.OrderDrinkConverstation(drinkRecipe.DrinkName)));
            orderDrink.Add(new StartDrinkOrderAction(new DrinkOrder
            {
                OrdererName = entity.GetState<NameState>().Name,
                OrdererSpecies = "Human",
                Recipe = drinkRecipe
            }));
            orderDrink.Add(WaitForDrink(entity, drinkRecipe, orderTimeoutInMins));
            return orderDrink;
        }

        public static ConditionalActionSequence GoToPaypointOrderDrinkAndSitDown(Entity entity, DrinkRecipe drinkRecipe, int orderTimeoutInMins = 20)
        {
            var orderingAndDrinking = new ConditionalActionSequence("OrderingAndDrinking");
            
            orderingAndDrinking.Add(GoToPaypointAndOrderDrink(entity, drinkRecipe, orderTimeoutInMins));            
            orderingAndDrinking.Add(SitDown());

            return orderingAndDrinking;
        }

        public static ActionSequence SitDown()
        {
            var sitDown = new ActionSequence("Sit down");
            sitDown.Add(new GetWaypointAction(Goal.Sit, reserve: true, closest: true)); //This assumes more seats than NPCs!
            sitDown.Add(new GoToWaypointAction());
            sitDown.Add(new TriggerAnimationAction(AnimationEvent.SittingStartTrigger));
            sitDown.Add(new PauseAction(6.0f));
            sitDown.Add(new TriggerAnimationAction(AnimationEvent.ChairTalk1Trigger));
            sitDown.Add(new PauseAction(4.0f));
            sitDown.Add(new TriggerAnimationAction(AnimationEvent.ChairTalk1Trigger));
            sitDown.Add(new PauseAction(3.0f));
            sitDown.Add(new TriggerAnimationAction(AnimationEvent.SittingFinishTrigger));
            sitDown.Add(new PauseAction(1.0f)); //Delay for standing up.
            sitDown.Add(new DrinkItemInInventory());
            sitDown.Add(new ReleaseWaypointAction());
            sitDown.Add(new GetWaypointAction(Goal.Storage, reserve: false, closest: true));
            sitDown.Add(new PutDownInventoryItemAtWaypoint());
            sitDown.Add(Wander());
            return sitDown;
        }

        public static ActionSequence PlayerUseBar()
        {
            var useBar = new ActionSequence("Use bar");
            useBar.Add(new GetWaypointAction(Goal.RingUp, reserve: true));
            useBar.Add(new GoToWaypointAction());
            useBar.Add(new StartUsingWaypointAction()); //TODO: Need to release this.
            useBar.Add(new MakeDrinkAction());
            return useBar;
        }
    }
}
