using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Dialogue;
using Assets.Scripts.Util.NPC;
using UnityEngine;
using AnimationEvent = Assets.Scripts.Util.AnimationEvent;

namespace Assets.Scripts.GameActions.Cutscenes
{
    static class PartyScene
    {
        public static void Start(List<Entity> matchingEntities) {

            var mcGraw = EntityQueries.GetEntityWithName(matchingEntities, NPCName.McGraw);
            var ellie = EntityQueries.GetEntityWithName(matchingEntities, NPCName.Ellie);
            var tolstoy = EntityQueries.GetEntityWithName(matchingEntities, NPCName.Tolstoy);
            var player = EntityQueries.GetEntityWithName(matchingEntities, NPCName.You);
            var q = EntityQueries.GetEntityWithName(matchingEntities, NPCName.Q);
            var jannet = EntityQueries.GetEntityWithName(matchingEntities, NPCName.Jannet);

            //Player
            var playerSequence = new ActionSequence("Player Party");
            playerSequence.Add(new TeleportAction(Locations.StandPoint1()));
            ActionManagerSystem.Instance.QueueAction(player, playerSequence);

            //Jannet
            if (jannet.GetState<RelationshipState>().PlayerOpinion > 0)
            {
                var jannetSequence = new ActionSequence("Jannet Party Positive");
                jannetSequence.Add(new TeleportAction(Locations.StandPoint2()));
                jannetSequence.Add(new SetReactiveConversationAction(new JannetPartyPositive(), jannet));
                jannetSequence.Add(new PauseAction(1.0f));
                jannetSequence.Add(CheerLoop());
                ActionManagerSystem.Instance.QueueAction(jannet, jannetSequence);
            }
            else
            {
                var jannetSequence = new ActionSequence("Jannet Party Negative");
                jannetSequence.Add(new TeleportAction(Locations.SitDownPoint1()));
                jannetSequence.Add(new SetReactiveConversationAction(new JannetPartyNegative(), jannet));
                ActionManagerSystem.Instance.QueueAction(jannet, jannetSequence);
            }

            //McGraw
            var mcGrawSequence = new ActionSequence("McGraw Party");
            mcGrawSequence.Add(new TeleportAction(Locations.StandPoint3()));
            mcGrawSequence.Add(new SetReactiveConversationAction(new JannetPartyPositive(), mcGraw));
            mcGrawSequence.Add(new PauseAction(2f));
            mcGrawSequence.Add(CheerLoop());
            ActionManagerSystem.Instance.QueueAction(mcGraw, mcGrawSequence);

            //Q
            var qSequence = new ActionSequence("Q Party");
            qSequence.Add(new TeleportAction(Locations.StandPoint4()));
            qSequence.Add(new SetReactiveConversationAction(new JannetPartyPositive(), q));
            qSequence.Add(new PauseAction(0.5f));
            qSequence.Add(CheerLoop());
            ActionManagerSystem.Instance.QueueAction(q, qSequence);

            //Tolstoy
            var tolstoySequence = new ActionSequence("Tolstoy Party");
            tolstoySequence.Add(new TeleportAction(Locations.StandPoint5()));
            tolstoySequence.Add(new SetReactiveConversationAction(new JannetPartyPositive(), tolstoy));
            tolstoySequence.Add(new PauseAction(3f));
            tolstoySequence.Add(CheerLoop());
            ActionManagerSystem.Instance.QueueAction(tolstoy, tolstoySequence);

            //Ellie
            var ellieSequence = new ActionSequence("Ellie Party");
            ellieSequence.Add(new TeleportAction(Locations.StandPoint6()));
            ellieSequence.Add(new SetReactiveConversationAction(new JannetPartyPositive(), ellie));
            ellieSequence.Add(CheerLoop());
            ActionManagerSystem.Instance.QueueAction(ellie, ellieSequence);
        }

        private static ConditionalActionSequence CheerLoop()
        {
            var cheer = new ConditionalActionSequence("Cheer");
            if (Random.value < 0.50f)
            {
                cheer.Add(new TriggerAnimationAction(AnimationEvent.Cheer3Trigger));
            }
            else if (Random.value < 0.75f)
            {
                cheer.Add(new TriggerAnimationAction(AnimationEvent.Cheer1Trigger));
            }
            else
            {
                cheer.Add(new TriggerAnimationAction(AnimationEvent.Cheer2Trigger));
            }
            cheer.Add(new PauseAction(4.0f));
            cheer.Add(new CallbackAction(() => cheer.Add(CheerLoop()))); //Lol
            return cheer;
        }

        private class JannetPartyPositive : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Jannet");
                DialogueSystem.Instance.WriteNPCLine("You are the best.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'll look into that.", EndConversation(DialogueOutcome.Nice));
            }
        }

        private class JannetPartyNegative : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Jannet");
                DialogueSystem.Instance.WriteNPCLine("You are the worst.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'll look into that.", EndConversation(DialogueOutcome.Nice));
            }
        }
    }
}
