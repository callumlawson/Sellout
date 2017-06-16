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
            var player = EntityQueries.GetEntityWithName(matchingEntities, "You");

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
            ellieSequence.Add(new SetConversationAction(new EllieMorningOne()));
            ellieSequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(ellie, ellieSequence);

            //Tolstoy
            var tolstoySequence = new ActionSequence("Tolstoy morning");
            tolstoySequence.Add(new PauseAction(0.5f)); //WORKAROUND FOR SYNC ACTION BUG
            tolstoySequence.Add(new TeleportAction(Locations.SitDownPoint2()));
            tolstoySequence.Add(new SetConversationAction(new TolstoyMorningOne()));
            tolstoySequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(tolstoy, tolstoySequence);
        }

        private class TolstoyMorningOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Tolstoy");
                DialogueSystem.Instance.WriteNPCLine("It's day two morning!.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Riiiiight...", EndConversation(DialogueOutcome.Default));
            }
        }

        private class EllieMorningOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Ellie");
                DialogueSystem.Instance.WriteNPCLine("The like the second day.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Sure.", EndConversation(DialogueOutcome.Mean));
            }
        }
    }
}
