using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.GameActions.DayPhases;
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

        private PlayerState playerState;

        public void OnInit()
        {
            EventSystem.onClickInteraction += OnClickInteraction;
            player = StaticStates.Get<PlayerState>().Player;
            playerState = StaticStates.Get<PlayerState>();
        }

        private void OnClickInteraction(ClickEvent clickevent)
        {
            if (DialogueSystem.Instance.ConverstationActive)
            {
                return;
            }

            if (!ActionManagerSystem.Instance.IsEntityIdle(player) && !playerState.TutorialControlLock)
            {
                ActionManagerSystem.Instance.TryClearActionsForEntity(player);
            }

            if (!ActionManagerSystem.Instance.IsEntityIdle(player))
            {
                return;
            }

            if (playerState.IsUsingBar)
            {
                return;
            }
            
            var targetEntity = clickevent.Target;
            if (targetEntity != null && targetEntity.HasState<PrefabState>())
            {
                var prefab = targetEntity.GetState<PrefabState>();
                if (!playerState.TutorialControlLock || (playerState.TutorialControlLock && prefab.PrefabName == Prefabs.Counter))
                {
                    ActionManagerSystem.Instance.QueueAction(player, new ReleaseWaypointAction());
                    QueueActionsForPrefab(targetEntity, prefab.PrefabName);
                }
            }
            else if (!playerState.TutorialControlLock)
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
                    ActionManagerSystem.Instance.QueueAction(player, CommonActions.PlayerUseBar());
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
                        ActionManagerSystem.Instance.QueueAction(player, TalkToNPC(targetEntity, isCancellable: true));
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
                case Prefabs.Console:
                    ActionManagerSystem.Instance.QueueAction(player, new GoToPositionAction(targetEntity.GetState<PositionState>().Position, 1.0f));
                    ActionManagerSystem.Instance.QueueAction(player, new ChangeDayPhaseAction());
                    break;
                case Prefabs.BarConsole:
                    ActionManagerSystem.Instance.QueueAction(player, new CloseBarIfOpenAction());
                    break;
            }
        }

        private static ActionSequence TalkToNPC(Entity targetEntity, bool isCancellable = true)
        {
            var actions = new ActionSequence("Talk to Person", isCancellable);
            actions.Add(new SetTargetEntityAction(targetEntity));
            actions.Add(new GoToMovingEntityAction());
            actions.Add(new PauseTargetActionSequeunceAction(targetEntity));
            if (targetEntity.HasState<ConversationState>())
            {
                var convo = targetEntity.GetState<ConversationState>().Conversation;
                if (convo != null)
                {
                    actions.Add(new ConversationAction(convo));
                }
            }
            else
            {
                actions.Add(Random.value > 0.4 ? new ConversationAction(Dialogues.DialogueOne) : new ConversationAction(Dialogues.DialogueTwo));
            }
            actions.Add(new UnpauseTargetActionSequeunceAction(targetEntity));
            return actions;
        }
    }
}
