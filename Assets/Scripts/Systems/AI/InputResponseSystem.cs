using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Framework.Util;
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
using Random = UnityEngine.Random;

namespace Assets.Scripts.Systems.AI
{
    class InputResponseSystem : IInitSystem, IFrameEntitySystem, IEntityManager
    {
        private static Entity player;
        private EntityStateSystem entitySystem;
        private PlayerState playerState;

        public void OnInit()
        {
            EventSystem.onClickInteraction += OnClickInteraction;
            player = StaticStates.Get<PlayerState>().Player;
            playerState = StaticStates.Get<PlayerState>();
        }

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(IsPlayerState) };
        }

        public void OnFrame(List<Entity> matchingEntities)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                var playerEntity = matchingEntities.First();
                var playerPosition = playerEntity.GameObject.transform.position;
                var colliders = Physics.OverlapSphere(playerPosition, Constants.InteractRangeInMeters);
                if (!colliders.Any())
                {
                    return;
                }
                var entityIds = colliders.Select(collider => collider.gameObject.GetEntityIdRecursive());
                var entities = entityIds.Where(entityId => entityId != EntityIdComponent.InvalidEntityId).Select(entityId => entitySystem.GetEntity(entityId)).ToList();
                entities = entities.Where(entity => !Equals(entity, StaticStates.Get<PlayerState>().Player)).ToList();
                if (!entities.Any())
                {
                    return;
                }
                var targetEntity = entities.First();
                Debug.Log("Target entity: " + targetEntity);
                OnInteraction(targetEntity);
            }
        }

        private void OnClickInteraction(ClickEvent clickevent)
        {
            var targetEntity = clickevent.Target;
            OnInteraction(targetEntity);
        }

        private void OnInteraction(Entity targetEntity)
        {
            if (DialogueSystem.Instance.ConverstationActive)
            {
                return;
            }

            if (!ActionManagerSystem.Instance.IsEntityIdle(player) && !playerState.CutsceneControlLock)
            {
                ActionManagerSystem.Instance.TryClearActionsForEntity(player);
            }

            if (!ActionManagerSystem.Instance.IsEntityIdle(player))
            {
                return;
            }

            if (playerState.PlayerStatus == PlayerStatus.Bar)
            {
                return;
            }

            if (targetEntity != null && targetEntity.HasState<PrefabState>())
            {
                var prefab = targetEntity.GetState<PrefabState>();
                if (!playerState.CutsceneControlLock || (playerState.CutsceneControlLock && prefab.PrefabName == Prefabs.Counter))
                {
                    ActionManagerSystem.Instance.QueueAction(player, new ReleaseWaypointAction());
                    QueueActionsForPrefab(targetEntity, prefab.PrefabName);
                }
            }
        }

        private static void QueueActionsForPrefab(Entity targetEntity, string prefab)
        {
            switch (prefab)
            {
                case Prefabs.Counter:
                case Prefabs.BeerStack:
                case Prefabs.GlassStack:
                case Prefabs.IngredientDispenser:
                case Prefabs.MixologyBook:
                    ActionManagerSystem.Instance.QueueAction(player, new TeleportAction(Locations.BehindBarLocation()));
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
                    ActionManagerSystem.Instance.QueueAction(player, new SetTargetEntityAction(targetEntity));
                    ActionManagerSystem.Instance.QueueAction(player, new PickUpItem());
                    break;
                case Prefabs.Washup:
                    ActionManagerSystem.Instance.QueueAction(player, new DestoryEntityInInventoryAction());
                    break;
                case Prefabs.Console:
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
