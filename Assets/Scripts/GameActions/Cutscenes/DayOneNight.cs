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

            var player = EntityQueries.GetEntityWithName(matchingEntities, NPCName.You);

            var seats = EntityStateSystem.Instance.GetEntitiesWithState<GoalSatisfierState>().Where(entity => entity.GetState<GoalSatisfierState>().SatisfiedGoals.Contains(Goal.Sit));
            var chosenSeats = seats.PickRandom(4).ToArray();

            //Jannet
            var jannetSequence = new ActionSequence("Jannet night");
            jannetSequence.Add(new CallbackAction(() =>
            {
                EventSystem.EndDrinkMakingEvent.Invoke();
                ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(player,
                    new TeleportAction(Locations.CenterOfBar()));
            })); //This is kind of dirty - but demo!
            jannetSequence.Add(new TeleportAction(chosenSeats[0].GameObject.transform));
            jannetSequence.Add(new SetConversationAction(new JannetNightOne(), jannet));
            jannetSequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(jannet, jannetSequence);

            //Tolstoy
            var tolstoySequence = new ActionSequence("Tolstoy night");
            tolstoySequence.Add(new TeleportAction(chosenSeats[1].GameObject.transform));
            tolstoySequence.Add(new SetConversationAction(new TolstoyNightOne(), tolstoy));
            tolstoySequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(tolstoy, tolstoySequence);

            //Ellie
            var ellieSequence = new ActionSequence("Ellie night");
            ellieSequence.Add(new TeleportAction(chosenSeats[2].GameObject.transform));
            ellieSequence.Add(new SetConversationAction(new EllieNightOne(), ellie));
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
                qSequence.Add(new TeleportAction(chosenSeats[3].GameObject.transform));
                qSequence.Add(new SetConversationAction(new QNightOneRefused(), q));
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

        private class QNightOneRefused : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Q");
                DialogueSystem.Instance.WriteNPCLine("You missed out on a great opportunity today.");
                DialogueSystem.Instance.WritePlayerChoiceLine("...", EndConversation(DialogueOutcome.Mean));
            }
        }

        private class JannetNightOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Jannet");
                if (StaticStates.Get<PlayerDecisionsState>().AcceptedDrugPushersOffer)
                {
                    DialogueSystem.Instance.WriteNPCLine("Q gave me a great deal on his, erm, plants.");
                    DialogueSystem.Instance.WriteNPCLine("It's all had to be so hush hush before. Did they change the rules");
                    DialogueSystem.Instance.WritePlayerChoiceLine("Something like that.", EndConversation(DialogueOutcome.Default));
                }
                else
                {
                    DialogueSystem.Instance.WriteNPCLine("What do you want?");
                    DialogueSystem.Instance.WritePlayerChoiceLine("Nothing, sorry.", EndConversation(DialogueOutcome.Default));
                }
                
            }
        }

        private class TolstoyNightOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Tolstoy");
                if (StaticStates.Get<PlayerDecisionsState>().GaveTolstoyDrink)
                {
                    DialogueSystem.Instance.WriteNPCLine("Handing me that drink this morning was a little thing.");
                    DialogueSystem.Instance.WriteNPCLine("But you don't even know me, don't owe me anything.");
                    DialogueSystem.Instance.WriteNPCLine("Dave wouldn't have done that.");
                    DialogueSystem.Instance.WriteNPCLine("Thank you.");
                    DialogueSystem.Instance.WritePlayerChoiceLine("It was nothing, glad to help.", EndConversation(DialogueOutcome.Nice));
                    DialogueSystem.Instance.WritePlayerChoiceLine("A happy customer comes back.", EndConversation(DialogueOutcome.Default));
                }
                else
                {
                    DialogueSystem.Instance.WriteNPCLine("I know it's late. I'm finishing up, don't worry.");
                    DialogueSystem.Instance.WritePlayerChoiceLine("I'm not in a rush - take your time.", EndConversation(DialogueOutcome.Nice));
                    DialogueSystem.Instance.WritePlayerChoiceLine("You have 10 minutes.", EndConversation(DialogueOutcome.Nice));
                }
            }
        }

        private class EllieNightOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Ellie");
                DialogueSystem.Instance.WriteNPCLine("When I ask for a drink containing my favorite ingredient I don't always want the same thing.");
                DialogueSystem.Instance.WriteNPCLine("That would be boring.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'll keep that in mind.", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("Why don't you just order exactly what you want!", EndConversation(DialogueOutcome.Mean));
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
