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

namespace Assets.Scripts.GameActions.Cutscenes
{
    static class DayTwoNight
    {
        public static void Start(List<Entity> matchingEntities) {
           
            var mcGraw = EntityQueries.GetEntityWithName(matchingEntities, NPCS.McGraw.Name);
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
            jannetSequence.Add(new PauseAction(0.1f)); //WORKAROUND FOR SYNC ACTION BUG
            jannetSequence.Add(new TeleportAction(Locations.SitDownPoint1()));
            jannetSequence.Add(new SetReactiveConversationAction(new JannetNightTwo(), jannet));
            jannetSequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(jannet, jannetSequence);

            //McGraw and Q
            DrugStory.DrugPusherInspectorShowdown(mcGraw, q, Locations.SitDownPoint2());            
        }

        private class JannetNightTwo : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                var decision = StaticStates.Get<PlayerDecisionsState>();
                DialogueSystem.Instance.StartDialogue("Jannet");
                if (decision.AcceptedDrugPushersOffer)
                {
                    DialogueSystem.Instance.WriteNPCLine("Have you seen Q? He owes me a, err, drink...");
                    DialogueSystem.Instance.WritePlayerChoiceLine("You should really cut down on the 'drink'.", EndConversation(DialogueOutcome.Default));
                    DialogueSystem.Instance.WritePlayerChoiceLine("Well if you need a 'drink' I'm here every day.", EndConversation(DialogueOutcome.Nice));
                    DialogueSystem.Instance.WritePlayerChoiceLine("You shouldn't mix with Q, he's trouble.", EndConversation(DialogueOutcome.Mean));
                }
                else
                {
                    DialogueSystem.Instance.WriteNPCLine("What a bust up!");
                    DialogueSystem.Instance.WriteNPCLine("How Q managed to find a market for his horrible substances I'll never know.");
                    DialogueSystem.Instance.WritePlayerChoiceLine("You'd be suprised. Being stuck in this space can drives people crazy eventually.", EndConversation(DialogueOutcome.Nice));
                    DialogueSystem.Instance.WritePlayerChoiceLine("You'd think in a ship as small as this there would be no room for it.", EndConversation(DialogueOutcome.Nice));
                }
            }
        }
    }
}
