using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions;
using Assets.Scripts.GameActions.Composite;
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
                ActionManagerSystem.Instance.QueueActionForEntity(player, new ReleaseWaypointAction());
                var prefab = targetEntity.GetState<PrefabState>();
                QueueActionsForPrefab(targetEntity, prefab.PrefabName);
            }
            else
            {
                ActionManagerSystem.Instance.QueueActionForEntity(player, new ReleaseWaypointAction());
                ActionManagerSystem.Instance.QueueActionForEntity(player, new GoToPositionAction(clickevent.ClickPosition));
            }
        }

        private static void QueueActionsForPrefab(Entity targetEntity, string prefab)
        {
            switch (prefab)
            {
                case Prefabs.Counter:
                    ActionManagerSystem.Instance.QueueActionForEntity(player, new GetWaypointAction(Goal.RingUp, reserve: true));
                    ActionManagerSystem.Instance.QueueActionForEntity(player, new GoToWaypointAction());
                    ActionManagerSystem.Instance.QueueActionForEntity(player, new StartUsingWaypointAction()); //TODO: Need to release this.
                    ActionManagerSystem.Instance.QueueActionForEntity(player, new MakeDrinkAction());
                    break;
                case Prefabs.Person:
                    var playerChild = player.GetState<HierarchyState>().Child;
                    var targetChild = targetEntity.GetState<HierarchyState>().Child;
                    if (playerChild != null && targetChild == null)
                    {
                        EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest { EntityFrom = player, EntityTo = targetEntity, Mover = playerChild });
                    }
                    else
                    {
                        ActionManagerSystem.Instance.QueueActionForEntity(player, TalkToPerson(targetEntity));
                    }
                    break;
                case Prefabs.Drink:
                    //TODO replace with GOTO entity
                    ActionManagerSystem.Instance.QueueActionForEntity(player, new GoToPositionAction(targetEntity.GetState<PositionState>().Position));
                    ActionManagerSystem.Instance.QueueActionForEntity(player, new SetTargetEntityAction(targetEntity));
                    ActionManagerSystem.Instance.QueueActionForEntity(player, new PickUpItem());
                    break;
                case Prefabs.Washup:
                    ActionManagerSystem.Instance.QueueActionForEntity(player, new GoToPositionAction(targetEntity.GetState<PositionState>().Position));
                    ActionManagerSystem.Instance.QueueActionForEntity(player, new DestoryEntityInInventoryAction()); 
                    break;
                default:
                    break;
            }
        }

        private static ActionSequence TalkToPerson(Entity targetEntity)
        {
            var actions = new ActionSequence("Talk to Person");
            actions.Add(new SetTargetEntityAction(targetEntity));
            actions.Add(new GoToMovingEntityAction(2.0f));
            actions.Add(new PauseTargetActionSequeunceAction(targetEntity));
            actions.Add(Random.value > 0.4 ? new ConversationAction(Dialogues.DialogueOne) : new ConversationAction(Dialogues.DialogueTwo));
            actions.Add(new UnpauseTargetActionSequeunceAction(targetEntity));
            return actions;
        }
    }
}
