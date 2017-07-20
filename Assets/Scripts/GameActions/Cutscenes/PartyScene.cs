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
    /**
     * The Crew assemble in the Bar to throw you a welcome party. 
     * Who's cheering and who's sulking depends on the decisions taken up to this point. 
     */
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

            //Either McGraw or Q can be your friend at the end. Not both.
            if (mcGraw.GetState<RelationshipState>().PlayerOpinion > 0)
            {
                var mcGrawSequence = new ActionSequence("McGraw Party");
                mcGrawSequence.Add(new TeleportAction(Locations.StandPoint3()));
                mcGrawSequence.Add(new SetReactiveConversationAction(new McGrawPartyPositive(), mcGraw));
                mcGrawSequence.Add(new PauseAction(2f));
                mcGrawSequence.Add(CheerLoop());
                ActionManagerSystem.Instance.QueueAction(mcGraw, mcGrawSequence);

                var qSequence = new ActionSequence("Q Party");
                qSequence.Add(new TeleportAction(Locations.SitDownPoint1()));
                qSequence.Add(new SetReactiveConversationAction(new QPartyNegative(), q));
                qSequence.Add(CommonActions.SitDownLoop());
                ActionManagerSystem.Instance.QueueAction(q, qSequence);
            }
            else
            {
                var qSequence = new ActionSequence("Q Party");
                qSequence.Add(new TeleportAction(Locations.StandPoint3()));
                qSequence.Add(new SetReactiveConversationAction(new QPartyPositive(), q));
                qSequence.Add(new PauseAction(0.5f));
                qSequence.Add(CheerLoop());
                ActionManagerSystem.Instance.QueueAction(q, qSequence);

                var mcGrawSequence = new ActionSequence("McGraw Party");
                mcGrawSequence.Add(new TeleportAction(Locations.SitDownPoint1()));
                mcGrawSequence.Add(new SetReactiveConversationAction(new McGrawPartyNegative(), mcGraw));
                mcGrawSequence.Add(CommonActions.SitDownLoop());
                ActionManagerSystem.Instance.QueueAction(mcGraw, mcGrawSequence);
            }

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
                jannetSequence.Add(new TeleportAction(Locations.SitDownPoint2()));
                jannetSequence.Add(new SetReactiveConversationAction(new JannetPartyNegative(), jannet));
                jannetSequence.Add(CommonActions.SitDownLoop());
                ActionManagerSystem.Instance.QueueAction(jannet, jannetSequence);
            }

            //Tolstoy
            if (tolstoy.GetState<RelationshipState>().PlayerOpinion > 0)
            {
                var tolstoySequence = new ActionSequence("Tolstoy Party");
                tolstoySequence.Add(new TeleportAction(Locations.StandPoint4()));
                tolstoySequence.Add(new SetReactiveConversationAction(new TolstoyPartyPositive(), tolstoy));
                tolstoySequence.Add(new PauseAction(3f));
                tolstoySequence.Add(CheerLoop());
                ActionManagerSystem.Instance.QueueAction(tolstoy, tolstoySequence);
            }
            else
            {
                var tolstoySequence = new ActionSequence("Tolstoy Party Negative");
                tolstoySequence.Add(new TeleportAction(Locations.SitDownPoint3()));
                tolstoySequence.Add(new SetReactiveConversationAction(new TolstoyPartyNegative(), tolstoy));
                tolstoySequence.Add(CommonActions.SitDownLoop());
                ActionManagerSystem.Instance.QueueAction(tolstoy, tolstoySequence);
            }

            //Ellies
            if (ellie.GetState<RelationshipState>().PlayerOpinion > 0)
            {
                var ellieSequence = new ActionSequence("Ellie Party");
                ellieSequence.Add(new TeleportAction(Locations.StandPoint5()));
                ellieSequence.Add(new SetReactiveConversationAction(new ElliePartyPositive(), ellie));
                ellieSequence.Add(CheerLoop());
                ActionManagerSystem.Instance.QueueAction(ellie, ellieSequence);
            }
            else
            {
                var ellieSequence = new ActionSequence("Tolstoy Party Negative");
                ellieSequence.Add(new TeleportAction(Locations.SitDownPoint4()));
                ellieSequence.Add(new SetReactiveConversationAction(new ElliePartyNegative(), ellie));
                ellieSequence.Add(CommonActions.SitDownLoop());
                ActionManagerSystem.Instance.QueueAction(ellie, ellieSequence);
            }
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

        private class QPartyPositive : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Q");
                DialogueSystem.Instance.WriteNPCLine("You are the best.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'll look into that.", EndConversation(DialogueOutcome.Nice));
            }
        }

        private class QPartyNegative : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Q");
                DialogueSystem.Instance.WriteNPCLine("That's to you I'm on probation. I could loose my Job!");
                DialogueSystem.Instance.WriteNPCLine("No one likes a snitch - Watch your back.");
                DialogueSystem.Instance.WritePlayerChoiceLine("What happened wasn't my fault. I even tried to protect you.", EndConversation(DialogueOutcome.Mean));
                DialogueSystem.Instance.WritePlayerChoiceLine("You got what you deserved. Those drugs hurt people.", EndConversation(DialogueOutcome.Mean));
            }
        }

        private class McGrawPartyPositive : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("McGraw");
                DialogueSystem.Instance.WriteNPCLine("You are the best.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'll look into that.", EndConversation(DialogueOutcome.Nice));
            }
        }

        private class McGrawPartyNegative : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("McGraw");
                DialogueSystem.Instance.WriteNPCLine("You are the wrost.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'll look into that.", EndConversation(DialogueOutcome.Nice));
            }
        }

        private class TolstoyPartyPositive : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Tolstoy");
                DialogueSystem.Instance.WriteNPCLine("You are the best.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'll look into that.", EndConversation(DialogueOutcome.Nice));
            }
        }

        private class TolstoyPartyNegative : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Tolstoy");
                DialogueSystem.Instance.WriteNPCLine("You are the wrost.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'll look into that.", EndConversation(DialogueOutcome.Nice));
            }
        }

        private class ElliePartyPositive : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Ellie");
                DialogueSystem.Instance.WriteNPCLine("You are the best.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'll look into that.", EndConversation(DialogueOutcome.Nice));
            }
        }

        private class ElliePartyNegative : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Ellie");
                DialogueSystem.Instance.WriteNPCLine("You are the wrost.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'll look into that.", EndConversation(DialogueOutcome.Nice));
            }
        }
    }
}
