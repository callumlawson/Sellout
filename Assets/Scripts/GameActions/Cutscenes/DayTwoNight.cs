using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.Systems;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Dialogue;
using Assets.Scripts.Util.NPC;

namespace Assets.Scripts.GameActions.Cutscenes
{
    static class DayTwoNight
    {
        public static void Start(List<Entity> matchingEntities) {
           
            var mcGraw = EntityQueries.GetEntityWithName(matchingEntities, NPCS.McGraw.Name);
            var jannet = EntityQueries.GetEntityWithName(matchingEntities, NPCS.Jannet.Name);
            var q = EntityQueries.GetEntityWithName(matchingEntities, NPCS.Q.Name);
            var player = EntityQueries.GetEntityWithName(matchingEntities, "You");

            //Jannet
            var jannetSequence = new ActionSequence("Jannet night");
            jannetSequence.Add(new CallbackAction(() =>
            {
                EventSystem.EndDrinkMakingEvent.Invoke();
                ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(player,
                    new TeleportAction(Locations.CenterOfBar()));
            })); //This is kind of dirty - but demo!
            jannetSequence.Add(new PauseAction(0.1f)); //WORKAROUND FOR SYNC ACTION BUG
            jannetSequence.Add(new TeleportAction(Locations.SitDownPoint1()));
            jannetSequence.Add(new SetConversationAction(new JannetNightOne()));
            jannetSequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(jannet, jannetSequence);

            //McGraw and Q
            DrugStory.DrugPusherInspectorShowdown(mcGraw, q, Locations.SitDownPoint2());            
        }

        private class QNightOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Q");
                DialogueSystem.Instance.WriteNPCLine("It's night on day 2.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Err, sure.", EndConversation(DialogueOutcome.Nice));
            }
        }

        private class McGrawNightOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("McGraw");
                DialogueSystem.Instance.WriteNPCLine("I like the second night.");
                DialogueSystem.Instance.WritePlayerChoiceLine("What's going on? What does placeholder mean?", EndConversation(DialogueOutcome.Nice));
            }
        }

        private class JannetNightOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Jannet");
                DialogueSystem.Instance.WriteNPCLine("In the jungle the mighty jungle...");
                DialogueSystem.Instance.WritePlayerChoiceLine("Riiiight.", EndConversation(DialogueOutcome.Nice));
            }
        }
    }
}
