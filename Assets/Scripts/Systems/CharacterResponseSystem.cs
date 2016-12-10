using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.GameActions.Waypoints;
using Assets.Scripts.States;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Dialogue;
using Assets.Scripts.Util.Events;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    class CharacterResponseSystem : IInitSystem
    {
        private static readonly DemoDialogueOne DialogueOne = new DemoDialogueOne();
        private static readonly DemoDialogueTwo DialogueTwo = new DemoDialogueTwo();

        private static Entity player; 

        public void OnInit()
        {
            EventSystem.onClickInteraction += OnClickInteraction;

            player = StaticStates.Get<PlayerState>().Player;
        }

        private static void OnClickInteraction(ClickEvent clickevent)
        {
            if (ActionManagerSystem.Instance.EntityHasActions(player))
            {
                Debug.Log("Player already has actions!");
                // Do this for now, then add cancelling...
                return;
            }
            
            var targetEntity = clickevent.target;
            if (targetEntity.HasState<PrefabState>())
            {
                var prefab = targetEntity.GetState<PrefabState>();
                QueueActionsForPrefab(targetEntity, prefab.PrefabName);
            }
        }

        private static void QueueActionsForPrefab(Entity targetEntity, string prefab)
        {
            ActionManagerSystem.Instance.QueueActionForEntity(player, new ReleaseWaypointAction());
            switch (prefab)
            {
                case Prefabs.Counter:
                    ActionManagerSystem.Instance.QueueActionForEntity(player, new GetAndReserveWaypointAction(Goal.RingUp));
                    ActionManagerSystem.Instance.QueueActionForEntity(player, new GoToWaypointAction());
                    ActionManagerSystem.Instance.QueueActionForEntity(player, new StartUsingWaypointAction()); //TODO: Need to release this.
                    ActionManagerSystem.Instance.QueueActionForEntity(player, new OpenDrinkMakingMenuAction());
                    break;
                case Prefabs.Person:
                    var playerInventory = player.GetState<InventoryState>();
                    var childToMove = playerInventory.child;
                    if (childToMove != null)
                    {
                        EventSystem.BroadcastEvent(new InventoryRequestEvent(player, targetEntity, childToMove));
                    }
                    else
                    {
                        ActionManagerSystem.Instance.QueueActionForEntity(player, TalkToPerson(targetEntity));
                    }
                    break;
            }
        }

        private static ActionSequence TalkToPerson(Entity targetEntity)
        {
            var actions = new ActionSequence();
            actions.Add(new GetEntityAction(targetEntity));
            actions.Add(new GoToMovingEntityAction(2.0f));
            actions.Add(new PauseTargetActionSequeunceAction(targetEntity));
            actions.Add(Random.value > 0.4 ? new ConversationAction(DialogueOne) : new ConversationAction(DialogueTwo));
            actions.Add(new UnpauseTargetActionSequeunceAction(targetEntity));
            return actions;
        }

        private class DemoDialogueOne : Conversation
        {
            protected override void StartConversation()
            {
                DialogueSystem.Instance.StartDialogue();
                DialogueSystem.Instance.WriteNPCLine("Hello there, what you up to?");
                DialogueSystem.Instance.WritePlayerChoiceLine("You're a bit friendly.", BitFriendly);
                DialogueSystem.Instance.WritePlayerChoiceLine("I'm running the bar now.", RunningBar);
                DialogueSystem.Instance.WritePlayerChoiceLine("Sorry, gotta wipe this up. Can't talk now.", EndConversation);
            }

            private void BitFriendly()
            {
                DialogueSystem.Instance.WriteNPCLine("It's a small boat. Friendly will get you far.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Fair point - thanks.", EndConversation);
            }

            private void RunningBar()
            {
                DialogueSystem.Instance.WriteNPCLine("Ah, shame about poor Fred... still, glad you'll have the taps flowing again.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'll do my best.", EndConversation);
            }
        }

        private class DemoDialogueTwo : Conversation
        {
            protected override void StartConversation()
            {
                DialogueSystem.Instance.StartDialogue();
                DialogueSystem.Instance.WriteNPCLine("What you looking at?");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'm looking at you.", Whoops);
            }

            private void Whoops()
            {
                DialogueSystem.Instance.WriteNPCLine("What you looking at <b>me</b> for?" );
                DialogueSystem.Instance.WritePlayerChoiceLine("I, err, don't know.", DiggingHole);
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>walk away</i>", EndConversation);
            }

            private void DiggingHole()
            {
                DialogueSystem.Instance.WriteNPCLine("Well sodd off then.");
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>walk quickly away</i>", EndConversation);
            }
        }
    }
}
