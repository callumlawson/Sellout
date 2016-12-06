using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.GameActions.Inventory;
using Assets.Scripts.GameActions.Waypoints;
using Assets.Scripts.States;
using Assets.Scripts.UI;
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
                    if (entity.GetState<NameState>().Name == "Tolstoy")
                    {
                        ActionManagerSystem.Instance.QueueActionForEntity(entity, OrderDrink());
                    }
                    else
                    {
                        ActionManagerSystem.Instance.QueueActionForEntity(entity, Wander());
                    }
                }
            }
        }

        private static ActionSequence OrderDrink()
        {
            var drink = new ActionSequence();
            drink.Add(new GetWaypointAction(Goal.PayFor));
            drink.Add(new GoToWaypointAction());
            drink.Add(new PauseAction(3.0f));
            drink.Add(new GetWaypointWithUserAction(Goal.RingUp, StaticStates.Get<PlayerState>().Player, 30));
            drink.Add(new ConversationAction(new OrderDrinkConversation()));
            drink.Add(new DrinkIsInInventoryAction(new DrinkState(DrinkUI.screwdriverIngredients), 30));
            drink.Add(new GetAndReserveWaypointAction(Goal.Sit));
            drink.Add(new GoToWaypointAction());
            drink.Add(new PauseAction(30.0f));
            drink.Add(new DestoryEntityInInventoryAction());
            drink.Add(new ReleaseWaypointAction());//TODO: Make this not required.
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
                wander.Add(new GetAndReserveWaypointAction(Goal.Sit));
                wander.Add(new GoToWaypointAction());
                wander.Add(new PauseAction(40.0f));
                wander.Add(new ReleaseWaypointAction());
            }
            return wander;
        }

        private class OrderDrinkConversation : Conversation
        {
            protected override void StartConversation()
            {
                DialogueSystem.Instance.StartDialogue();
                DialogueSystem.Instance.WriteNPCLine("Once Space Screwdriver please.");
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>Nod.</i>", EndConversation);
            }
        }
    }
}
