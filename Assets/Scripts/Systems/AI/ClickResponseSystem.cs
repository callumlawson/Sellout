﻿using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.GameActions.Dialogue;
using Assets.Scripts.GameActions.Inventory;
using Assets.Scripts.GameActions.Waypoints;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Dialogue;
using Assets.Scripts.Util.Events;
using UnityEngine;

namespace Assets.Scripts.Systems.AI
{
    class ClickResponseSystem : IInitSystem
    {
        private static Entity player; 

        public void OnInit()
        {
            EventSystem.onClickInteraction += OnClickInteraction;
            player = StaticStates.Get<PlayerState>().Player;
        }

        private static void OnClickInteraction(ClickEvent clickevent)
        {
            if (DialogueSystem.Instance.ConverstationActive)
            {
                return;
            }

            if (!ActionManagerSystem.Instance.IsEntityIdle(player))
            {
                ActionManagerSystem.Instance.TryClearActionsForEntity(player);
            }

            if (!ActionManagerSystem.Instance.IsEntityIdle(player))
            {
                return;
            }
            
            var targetEntity = clickevent.Target;
            if (targetEntity != null && targetEntity.HasState<PrefabState>())
            {
                ActionManagerSystem.Instance.QueueAction(player, new ReleaseWaypointAction());
                var prefab = targetEntity.GetState<PrefabState>();
                QueueActionsForPrefab(targetEntity, prefab.PrefabName);
            }
            else
            {
                ActionManagerSystem.Instance.QueueAction(player, new ReleaseWaypointAction());
                ActionManagerSystem.Instance.QueueAction(player, new GoToPositionAction(clickevent.ClickPosition));
            }
        }

        private static void QueueActionsForPrefab(Entity targetEntity, string prefab)
        {
            switch (prefab)
            {
                case Prefabs.Counter:
                    ActionManagerSystem.Instance.QueueAction(player, new GetWaypointAction(Goal.RingUp, reserve: true));
                    ActionManagerSystem.Instance.QueueAction(player, new GoToWaypointAction());
                    ActionManagerSystem.Instance.QueueAction(player, new StartUsingWaypointAction()); //TODO: Need to release this.
                    ActionManagerSystem.Instance.QueueAction(player, new MakeDrinkAction());
                    break;
                case Prefabs.Person:
                    var playerChild = player.GetState<InventoryState>().Child;
                    var targetChild = targetEntity.GetState<InventoryState>().Child;
                    if (playerChild != null && targetChild == null)
                    {
                        EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest { EntityFrom = player, EntityTo = targetEntity, Mover = playerChild });
                    }
                    else
                    {
                        ActionManagerSystem.Instance.QueueAction(player, TalkToPerson(targetEntity, isCancellable: true));
                    }
                    break;
                case Prefabs.Drink:
                    //TODO replace with GOTO entity
                    ActionManagerSystem.Instance.QueueAction(player, new GoToPositionAction(targetEntity.GetState<PositionState>().Position));
                    ActionManagerSystem.Instance.QueueAction(player, new SetTargetEntityAction(targetEntity));
                    ActionManagerSystem.Instance.QueueAction(player, new PickUpItem());
                    break;
                case Prefabs.Washup:
                    ActionManagerSystem.Instance.QueueAction(player, new GoToPositionAction(targetEntity.GetState<PositionState>().Position));
                    ActionManagerSystem.Instance.QueueAction(player, new DestoryEntityInInventoryAction()); 
                    break;
                default:
                    break;
            }
        }

        private static ActionSequence TalkToPerson(Entity targetEntity, bool isCancellable = true)
        {
            var actions = new ActionSequence("Talk to Person", isCancellable);
            actions.Add(new SetTargetEntityAction(targetEntity));
            actions.Add(new GoToMovingEntityAction());
            actions.Add(new PauseTargetActionSequeunceAction(targetEntity));
            actions.Add(Random.value > 0.4 ? new ConversationAction(Dialogues.DialogueOne) : new ConversationAction(Dialogues.DialogueTwo));
            actions.Add(new UnpauseTargetActionSequeunceAction(targetEntity));
            return actions;
        }
    }
}
