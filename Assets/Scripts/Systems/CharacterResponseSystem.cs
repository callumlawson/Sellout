using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions;
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
        private static DemoDialogueOne dialogueOne = new DemoDialogueOne();
        private static DemoDialogueTwo dialogueTwo = new DemoDialogueTwo();

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
            switch (prefab)
            {
                case Prefabs.Counter:
                    ActionManagerSystem.Instance.QueueActionForEntity(player, new GetWaypointAction(Goal.RingUp));
                    ActionManagerSystem.Instance.QueueActionForEntity(player, new GoToWaypointAction());
                    ActionManagerSystem.Instance.QueueActionForEntity(player, new OpenDrinkMakingMenuAction());
                    break;
                case Prefabs.Person:
                    ActionManagerSystem.Instance.QueueActionForEntity(player, new GetEntityAction(targetEntity));
                    ActionManagerSystem.Instance.QueueActionForEntity(player, new GoToMovingEntityAction(1.0f));
                    ActionManagerSystem.Instance.QueueActionForEntity(player, new PauseTargetActionSequeunceAction(targetEntity));
                    if (Random.value > 0.4)
                    {
                        ActionManagerSystem.Instance.QueueActionForEntity(player, new ConversationAction(dialogueOne));
                    }
                    else
                    {
                        ActionManagerSystem.Instance.QueueActionForEntity(player, new ConversationAction(dialogueTwo));
                    }
                    ActionManagerSystem.Instance.QueueActionForEntity(player, new UnpauseTargetActionSequeunceAction(targetEntity));
                    break;
                default:
                    break;
            }
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
