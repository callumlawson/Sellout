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
    static class DayTwoMorning
    {
        public static void Start(List<Entity> matchingEntities) {

            var mcGraw = EntityQueries.GetEntityWithName(matchingEntities, NPCS.McGraw.Name);
            var ellie = EntityQueries.GetEntityWithName(matchingEntities, NPCS.Ellie.Name);
            var tolstoy = EntityQueries.GetEntityWithName(matchingEntities, NPCS.Tolstoy.Name);
            var player = EntityQueries.GetEntityWithName(matchingEntities, NPCName.You);

            //McGraw
            var mcGrawSequence = new ActionSequence("McGrawTutorial");
            mcGrawSequence.Add(new CallbackAction(() =>
            {
                EventSystem.EndDrinkMakingEvent.Invoke();
                ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(player,
                    new TeleportAction(Locations.CenterOfBar()));
            })); //This is kind of dirty - but demo!
            mcGrawSequence.Add(new PauseAction(2.0f)); //WORKAROUND FOR SYNC ACTION BUG
            mcGrawSequence.Add(DrugStory.InspectorQuestions(mcGraw));
            ActionManagerSystem.Instance.QueueAction(mcGraw, mcGrawSequence);

            //Ellie
            var ellieSequence = new ActionSequence("Ellie morning");
            ellieSequence.Add(new PauseAction(0.5f)); //WORKAROUND FOR SYNC ACTION BUG
            ellieSequence.Add(new TeleportAction(Locations.SitDownPoint1()));
            ellieSequence.Add(new SetReactiveConversationAction(new EllieMorningOne(), ellie));
            ellieSequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(ellie, ellieSequence);

            //Tolstoy
            var tolstoySequence = new ActionSequence("Tolstoy morning");
            tolstoySequence.Add(new PauseAction(0.5f)); //WORKAROUND FOR SYNC ACTION BUG
            tolstoySequence.Add(new TeleportAction(Locations.SitDownPoint2()));
            tolstoySequence.Add(new SetReactiveConversationAction(new TolstoyMorningOne(), tolstoy));
            tolstoySequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(tolstoy, tolstoySequence);
        }

        private class TolstoyMorningOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Tolstoy");
                DialogueSystem.Instance.WriteNPCLine("You really need to get window shields.");
                DialogueSystem.Instance.WriteNPCLine("This horrible purple light makes my drinks look wierd.");
                DialogueSystem.Instance.WritePlayerChoiceLine("How can you not want to look at the stars?", EndConversation(DialogueOutcome.Mean));
                DialogueSystem.Instance.WritePlayerChoiceLine("I'll look into that.", EndConversation(DialogueOutcome.Nice));
            }
        }

        private class EllieMorningOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Ellie");
                DialogueSystem.Instance.WriteNPCLine("I love this purple nebula.");
                DialogueSystem.Instance.WriteNPCLine("Imagine the strange new worlds it could be hiding!");
                DialogueSystem.Instance.WritePlayerChoiceLine("It is beautiful.", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("I've never really cared much for space.", EndConversation(DialogueOutcome.Mean));
            }
        }
    }
}
