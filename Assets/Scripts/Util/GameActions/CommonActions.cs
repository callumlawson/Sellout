using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.GameActions.Decorators;
using Assets.Scripts.GameActions.Dialogue;
using Assets.Scripts.GameActions.Inventory;
using Assets.Scripts.GameActions.Waypoints;
using Assets.Scripts.States;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util.Dialogue;
using UnityEngine;

namespace Assets.Scripts.Util.GameActions
{
    class CommonActions
    {
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
                wander.Add(new PauseAction(40.0f));
                wander.Add(new ReleaseWaypointAction());
            }
            return wander;
        }

        public static ConditionalActionSequence OrderDrink(Entity entity, DrinkRecipe drinkRecipe)
        {
            var orderDrink = new ConditionalActionSequence("OrderDrinkIfPossible");

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
                    ActionManagerSystem.Instance.QueueActionForEntity(entity, CommonActions.Wander());
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

            return orderDrink;
        }

        public static ActionSequence OrderDrinkAndSitDown(Entity entity, DrinkRecipe drinkRecipe)
        {
            var orderingAndDrinking = new ActionSequence("OrderingAndDrinking");
            var orderDrink = OrderDrink(entity, drinkRecipe);
            var sitDown = new ActionSequence("Sit down");
            
            orderingAndDrinking.Add(orderDrink);
            orderingAndDrinking.Add(sitDown);
            orderingAndDrinking.Add(new ReleaseWaypointAction());

            sitDown.Add(new GetWaypointAction(Goal.Sit, reserve: true, closest: true));
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
