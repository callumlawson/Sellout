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
using Assets.Scripts.GameActions.Stories;

namespace Assets.Scripts.GameActions.Cutscenes
{
    static class DayOneNight
    {
        public static void Start(List<Entity> matchingEntities) {

            var seats = EntityStateSystem.Instance.GetEntitiesWithState<GoalSatisfierState>().Where(entity => entity.GetState<GoalSatisfierState>().SatisfiedGoals.Contains(Goal.Sit));
            var chosenSeats = seats.PickRandom(4).ToArray();

            var loveStoryActions = LoveStory.DayOneNight(chosenSeats);
            foreach (var actionPair in loveStoryActions)
            {
                var entity = actionPair.GetEntity();
                var action = actionPair.GetGameAction();
                ActionManagerSystem.Instance.QueueAction(entity, action);
            }

            var jannet = EntityQueries.GetEntityWithName(matchingEntities, NPCS.Jannet.Name);
            var q = EntityQueries.GetEntityWithName(matchingEntities, NPCS.Q.Name);

            var player = EntityQueries.GetEntityWithName(matchingEntities, NPCName.You);

            //Jannet
            var jannetSequence = new ActionSequence("Jannet night");
            jannetSequence.Add(new CallbackAction(() =>
            {
                EventSystem.EndDrinkMakingEvent.Invoke();
                ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(player,
                    new TeleportAction(Locations.CenterOfBar()));
            })); //This is kind of dirty - but demo!
            jannetSequence.Add(new TeleportAction(chosenSeats[0].GameObject.transform));
            jannetSequence.Add(new SetReactiveConversationAction(new JannetNightOne(), jannet));
            jannetSequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(jannet, jannetSequence);
            
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
                qSequence.Add(new SetReactiveConversationAction(new QNightOneRefused(), q));
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
            getPayed.Add(new ModifyMoneyAction(100, PaymentType.DrugMoney));
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
