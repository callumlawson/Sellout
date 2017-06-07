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
    static class DayOneNight
    {
        public static void Start(List<Entity> matchingEntities) {
           
            var mcGraw = EntityQueries.GetEntityWithName(matchingEntities, NPCS.McGraw.Name);
            var jannet = EntityQueries.GetEntityWithName(matchingEntities, NPCS.Jannet.Name);
            var q = EntityQueries.GetEntityWithName(matchingEntities, NPCS.Q.Name);
            var player = EntityQueries.GetEntityWithName(matchingEntities, "You");

            //McGraw
            var mcGrawSequence = new ActionSequence("McGrawTutorial");
            mcGrawSequence.Add(new CallbackAction(() =>
            {
                EventSystem.EndDrinkMakingEvent.Invoke();
                ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(player,
                    new TeleportAction(Locations.CenterOfBar()));
            })); //This is kind of dirty - but demo!
            mcGrawSequence.Add(new TeleportAction(Locations.SitDownPoint3()));
            mcGrawSequence.Add(new SetConversationAction(new McGrawNightOne()));
            mcGrawSequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(mcGraw, mcGrawSequence);

            //Jannet
            var jannetSequence = new ActionSequence("Jannet night");
            jannetSequence.Add(new PauseAction(0.1f)); //WORKAROUND FOR SYNC ACTION BUG
            jannetSequence.Add(new TeleportAction(Locations.SitDownPoint1()));
            jannetSequence.Add(new SetConversationAction(new JannetNightOne()));
            jannetSequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(jannet, jannetSequence);

            //Q
            var qSequence = new ActionSequence("Q night");
            qSequence.Add(new PauseAction(0.2f)); //WORKAROUND FOR SYNC ACTION BUG
            qSequence.Add(new TeleportAction(Locations.SitDownPoint2()));
            qSequence.Add(new SetConversationAction(new QNightOne()));
            qSequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(q, qSequence);
        }

        private class QNightOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Q");
                DialogueSystem.Instance.WriteNPCLine("Placeholder.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Err, sure.", EndConversation(DialogueOutcome.Nice));
            }
        }

        private class McGrawNightOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("McGraw");
                DialogueSystem.Instance.WriteNPCLine("Placeholder.");
                DialogueSystem.Instance.WritePlayerChoiceLine("What's going on? What does placeholder mean?", EndConversation(DialogueOutcome.Nice));
            }
        }

        private class JannetNightOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Jannet");
                DialogueSystem.Instance.WriteNPCLine("Placeholder.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Riiiight.", EndConversation(DialogueOutcome.Nice));
            }
        }
    }
}
