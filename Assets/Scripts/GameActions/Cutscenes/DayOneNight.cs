using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Dialogue;
using Assets.Scripts.Util.NPC;
using Assets.Scripts.GameActions.AILifecycle;
using Assets.Framework.Systems;
using System.Linq;

namespace Assets.Scripts.GameActions.Cutscenes
{
    static class DayOneNight
    {
        public static void Start(List<Entity> matchingEntities) {
           
            var tolstoy = EntityQueries.GetEntityWithName(matchingEntities, NPCS.Tolstoy.Name);
            var jannet = EntityQueries.GetEntityWithName(matchingEntities, NPCS.Jannet.Name);
            var ellie = EntityQueries.GetEntityWithName(matchingEntities, NPCS.Ellie.Name);
            var q = EntityQueries.GetEntityWithName(matchingEntities, NPCS.Q.Name);

            var player = EntityQueries.GetEntityWithName(matchingEntities, "You");

            var seats = EntityStateSystem.Instance.GetEntitiesWithState<GoalSatisfierState>().Where(entity => entity.GetState<GoalSatisfierState>().SatisfiedGoals.Contains(Goal.Sit));
            var chosenSeats = seats.PickRandom(3).ToArray();

            //Jannet
            var jannetSequence = new ActionSequence("Jannet night");
            jannetSequence.Add(new CallbackAction(() =>
            {
                EventSystem.EndDrinkMakingEvent.Invoke();
                ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(player,
                    new TeleportAction(Locations.CenterOfBar()));
            })); //This is kind of dirty - but demo!
            jannetSequence.Add(new PauseAction(0.1f)); //WORKAROUND FOR SYNC ACTION BUG
            jannetSequence.Add(new TeleportAction(chosenSeats[0].GameObject.transform));
            jannetSequence.Add(new SetConversationAction(new JannetNightOne()));
            jannetSequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(jannet, jannetSequence);

            //Tolstoy
            var tolstoySequence = new ActionSequence("Tolstoy night");
            tolstoySequence.Add(new PauseAction(0.1f)); //WORKAROUND FOR SYNC ACTION BUG
            tolstoySequence.Add(new TeleportAction(chosenSeats[2].GameObject.transform));
            tolstoySequence.Add(new SetConversationAction(new TolstoyNightOne()));
            tolstoySequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(tolstoy, tolstoySequence);

            //Ellie
            var ellieSequence = new ActionSequence("Ellie night");
            ellieSequence.Add(new PauseAction(0.1f)); //WORKAROUND FOR SYNC ACTION BUG
            ellieSequence.Add(new TeleportAction(chosenSeats[3].GameObject.transform));
            ellieSequence.Add(new SetConversationAction(new EllieNightOne()));
            ellieSequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(ellie, ellieSequence);

            //Q
            var qSequence = new ActionSequence("Q night");
            qSequence.Add(new PauseAction(2.0f)); //WORKAROUND FOR SYNC ACTION BUG
            if (StaticStates.Get<PlayerDecisionsState>().AcceptedDrugPushersOffer)
            {   
                qSequence.Add(DrugPusherPaysYou());
            }
            else
            {
                qSequence.Add(new TeleportAction(chosenSeats[1].GameObject.transform));
                qSequence.Add(new SetConversationAction(new QNightOneRefused()));
                qSequence.Add(CommonActions.SitDownLoop());
            }
            ActionManagerSystem.Instance.QueueAction(q, qSequence);
        }

        private static ActionSequence DrugPusherPaysYou()
        {
            var getPayed = new ActionSequence("DrugPusherPaysYou");
            getPayed.Add(CommonActions.TalkToPlayer(new DrugPusherPayment()));
            getPayed.Add(new TriggerAnimationAction(AnimationEvent.ItemRecieveTrigger));
            getPayed.Add(new PauseAction(0.5f));
            getPayed.Add(new ModifyMoneyAction(100));
            getPayed.Add(new LeaveBarAction());
            return getPayed;
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

        private class QNightOneRefused : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Q");
                DialogueSystem.Instance.WriteNPCLine("You missed out on a great opportunity today.");
                DialogueSystem.Instance.WritePlayerChoiceLine("...", EndConversation(DialogueOutcome.Default));
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

        private class TolstoyNightOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Tolstoy");
                DialogueSystem.Instance.WriteNPCLine("Placeholder.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Riiiight.", EndConversation(DialogueOutcome.Nice));
            }
        }

        private class EllieNightOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Ellie");
                DialogueSystem.Instance.WriteNPCLine("Placeholder.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Riiiight.", EndConversation(DialogueOutcome.Nice));
            }
        }

        private class DrugPusherPayment : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                if (StaticStates.Get<PlayerDecisionsState>().AcceptedDrugPushersOffer)
                {
                    DialogueSystem.Instance.StartDialogue(converstationInitiator);
                    DialogueSystem.Instance.WriteNPCLine("Pretty good day today. Here is your cut.");
                    DialogueSystem.Instance.WritePlayerChoiceLine("Thanks.", EndConversation(DialogueOutcome.Nice));
                }
            }
        }
    }
}
