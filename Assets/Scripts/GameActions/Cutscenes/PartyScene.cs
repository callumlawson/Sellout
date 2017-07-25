using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.AILifecycle;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.GameActions.Dialogue;
using Assets.Scripts.GameActions.Waypoints;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Dialogue;
using Assets.Scripts.Util.NPC;
using Assets.Scripts.Visualizers;
using UnityEngine;
using AnimationEvent = Assets.Scripts.Util.AnimationEvent;

namespace Assets.Scripts.GameActions.Cutscenes
{
    /**
     * The Crew assemble in the Bar to throw you a welcome party. 
     * Who's cheering and who's sulking depends on the decisions taken up to this point. 
     * 
     * Either Q or McGraw gives a short speach - depending on your actions.
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

            var cheeringStartPoint = new SyncedAction(new List<Entity> { mcGraw, ellie, tolstoy, q, jannet});

            //Player
            var playerSequence = new ActionSequence("Player Party");
            playerSequence.Add(new CallbackAction(() => WelcomeSignControllerVisualizer.Instance.SetWelcomeSignActive(true)));
            playerSequence.Add(new TeleportAction(Locations.StandPoint1()));
            ActionManagerSystem.Instance.QueueAction(player, playerSequence);

            var mcGrawSequence = new ActionSequence("McGraw Party");
            var qSequence = new ActionSequence("Q Party");

            //Either McGraw or Q can be your friend at the end. Not both.
            if (mcGraw.GetState<RelationshipState>().PlayerOpinion >= 0)
            {
                mcGrawSequence.Add(new TeleportAction(Locations.StandPoint3()));
                mcGrawSequence.Add(new ConversationAction(new McGrawPartyGreeting()));
                mcGrawSequence.Add(cheeringStartPoint);
                mcGrawSequence.Add(new SetReactiveConversationAction(new McGrawPartyPositive(), mcGraw));
                mcGrawSequence.Add(new PauseAction(1f));
                mcGrawSequence.Add(CheerLoop(2));
                mcGrawSequence.Add(new GetWaypointAction(Goal.Sit, true, true));
                mcGrawSequence.Add(new GoToWaypointAction());
                mcGrawSequence.Add(CommonActions.SitDownLoop());
                ActionManagerSystem.Instance.QueueAction(mcGraw, mcGrawSequence);

                qSequence.Add(new TeleportAction(Locations.SitDownPoint1()));
                qSequence.Add(cheeringStartPoint);
                qSequence.Add(new SetReactiveConversationAction(new QPartyNegative(), q));
                qSequence.Add(CommonActions.SitDownLoop());
                ActionManagerSystem.Instance.QueueAction(q, qSequence);
            }
            else
            {
                qSequence.Add(new TeleportAction(Locations.StandPoint3()));
                qSequence.Add(new ConversationAction(new QPartyGreeting()));
                qSequence.Add(cheeringStartPoint);
                qSequence.Add(new SetReactiveConversationAction(new QPartyPositive(), q));
                qSequence.Add(new PauseAction(0.5f));
                qSequence.Add(CheerLoop(3));
                qSequence.Add(new GetWaypointAction(Goal.Sit, true, true));
                qSequence.Add(new GoToWaypointAction());
                qSequence.Add(CommonActions.SitDownLoop());
                ActionManagerSystem.Instance.QueueAction(q, qSequence);

                mcGrawSequence.Add(new TeleportAction(Locations.SitDownPoint1()));
                mcGrawSequence.Add(cheeringStartPoint);
                mcGrawSequence.Add(new SetReactiveConversationAction(new McGrawPartyNegative(), mcGraw));
                mcGrawSequence.Add(CommonActions.SitDownLoop());
                ActionManagerSystem.Instance.QueueAction(mcGraw, mcGrawSequence);
            }

            //Jannet
            if (jannet.GetState<RelationshipState>().PlayerOpinion >= 0)
            {
                var jannetSequence = new ActionSequence("Jannet Party Positive");
                jannetSequence.Add(new TeleportAction(Locations.StandPoint2()));
                jannetSequence.Add(cheeringStartPoint);
                jannetSequence.Add(new SetReactiveConversationAction(new JannetPartyPositive(), jannet));
                jannetSequence.Add(new PauseAction(1.2f));
                jannetSequence.Add(CheerLoop(3));
                jannetSequence.Add(new GetWaypointAction(Goal.Sit, true, true));
                jannetSequence.Add(new GoToWaypointAction());
                jannetSequence.Add(CommonActions.SitDownLoop());
                ActionManagerSystem.Instance.QueueAction(jannet, jannetSequence);
            }
            else
            {
                var jannetSequence = new ActionSequence("Jannet Party Negative");
                jannetSequence.Add(new TeleportAction(Locations.SitDownPoint2()));
                jannetSequence.Add(cheeringStartPoint);
                jannetSequence.Add(new SetReactiveConversationAction(new JannetPartyNegative(), jannet));
                jannetSequence.Add(CommonActions.SitDownLoop());
                ActionManagerSystem.Instance.QueueAction(jannet, jannetSequence);
            }

            //Tolstoy
            if (tolstoy.GetState<RelationshipState>().PlayerOpinion >= 0)
            {
                var tolstoySequence = new ActionSequence("Tolstoy Party");
                tolstoySequence.Add(new TeleportAction(Locations.StandPoint4()));
                tolstoySequence.Add(cheeringStartPoint);
                tolstoySequence.Add(new SetReactiveConversationAction(new TolstoyPartyPositive(), tolstoy));
                tolstoySequence.Add(new PauseAction(1f));
                tolstoySequence.Add(CheerLoop(2));
                tolstoySequence.Add(new GetWaypointAction(Goal.Sit, true, true));
                tolstoySequence.Add(new GoToWaypointAction());
                tolstoySequence.Add(CommonActions.SitDownLoop());
                ActionManagerSystem.Instance.QueueAction(tolstoy, tolstoySequence);
            }
            else
            {
                var tolstoySequence = new ActionSequence("Tolstoy Party Negative");
                tolstoySequence.Add(new TeleportAction(Locations.SitDownPoint3()));
                tolstoySequence.Add(cheeringStartPoint);
                tolstoySequence.Add(new SetReactiveConversationAction(new TolstoyPartyNegative(), tolstoy));
                tolstoySequence.Add(CommonActions.SitDownLoop());
                ActionManagerSystem.Instance.QueueAction(tolstoy, tolstoySequence);
            }

            //Ellies
            if (ellie.GetState<RelationshipState>().PlayerOpinion >= 0)
            {
                var ellieSequence = new ActionSequence("Ellie Party");
                ellieSequence.Add(new TeleportAction(Locations.StandPoint5()));
                ellieSequence.Add(cheeringStartPoint);
                ellieSequence.Add(new SetReactiveConversationAction(new ElliePartyPositive(), ellie));
                ellieSequence.Add(CheerLoop(3));
                ellieSequence.Add(new GetWaypointAction(Goal.Sit, true, true));
                ellieSequence.Add(new GoToWaypointAction());
                ellieSequence.Add(CommonActions.SitDownLoop());
                ActionManagerSystem.Instance.QueueAction(ellie, ellieSequence);
            }
            else
            {
                var ellieSequence = new ActionSequence("Tolstoy Party Negative");
                ellieSequence.Add(new TeleportAction(Locations.SitDownPoint4()));
                ellieSequence.Add(cheeringStartPoint);
                ellieSequence.Add(new SetReactiveConversationAction(new ElliePartyNegative(), ellie));
                ellieSequence.Add(CommonActions.SitDownLoop());
                ActionManagerSystem.Instance.QueueAction(ellie, ellieSequence);
            }
        }

        private static ConditionalActionSequence CheerLoop(int repetitions)
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
            if (repetitions > 1)
            {
                cheer.Add(new CallbackAction(() => cheer.Add(CheerLoop(repetitions - 1)))); //Lol    
            }
            return cheer;
        }

        private class JannetPartyPositive : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Jannet");
                DialogueSystem.Instance.WriteNPCLine("I much prefer you to the guy who worked here before.");
                DialogueSystem.Instance.WriteNPCLine("Glad to have you on board!");
                DialogueSystem.Instance.WritePlayerChoiceLine("Thank you!", EndConversation(DialogueOutcome.Nice));
            }
        }

        private class JannetPartyNegative : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Jannet");
                DialogueSystem.Instance.WriteNPCLine("I'm not celebrating.");
                DialogueSystem.Instance.WriteNPCLine("This bar was far better before you took over.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'm sorry, I'll try and improve things.", EndConversation(DialogueOutcome.Default));
                DialogueSystem.Instance.WritePlayerChoiceLine("Like I give a dam.", EndConversation(DialogueOutcome.Mean));
            }
        }

        private class QPartyGreeting : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("McGraw");
                DialogueSystem.Instance.WriteNPCLine("On behalf of the crew I would like to welcome you onboard!");
                DialogueSystem.Instance.WriteNPCLine("Thanks to you this is the finest party vessle in the Federation.");
                DialogueSystem.Instance.WriteNPCLine("And that boring McGraw has been put in his place! Three cheers!");
                DialogueSystem.Instance.WritePlayerChoiceLine("Let's get this party started!", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("Is he ok?", EndConversation(DialogueOutcome.Mean));
            }
        }

        private class QPartyPositive : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Q");
                DialogueSystem.Instance.WriteNPCLine("We have a tidy little operation going here.");
                DialogueSystem.Instance.WriteNPCLine("It will profit both of us - and gives the crew an important way to blow off steam.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Win, win. Pleasure doing business with you.", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("I regret it. McGraw is just trying to look after the crew.", EndConversation(DialogueOutcome.Mean));
            }
        }

        private class QPartyNegative : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Q");
                DialogueSystem.Instance.WriteNPCLine("Thank's to you I'm on probation. I could loose my Job!");
                DialogueSystem.Instance.WriteNPCLine("No one likes a snitch - Watch your back.");
                DialogueSystem.Instance.WritePlayerChoiceLine("What happened wasn't my fault. I even tried to protect you.", EndConversation(DialogueOutcome.Mean));
                DialogueSystem.Instance.WritePlayerChoiceLine("You got what you deserved. Those drugs hurt people.", EndConversation(DialogueOutcome.Mean));
            }
        }

        private class McGrawPartyGreeting : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("McGraw");
                DialogueSystem.Instance.WriteNPCLine("On behalf of the crew I would like to welcome you onboard!");
                DialogueSystem.Instance.WriteNPCLine("You've already shown yourself to be a great choice.");
                DialogueSystem.Instance.WriteNPCLine("Your predcessor wouldn't have handled Q like you did. To you!");
                DialogueSystem.Instance.WritePlayerChoiceLine("It was nothing!", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("<i> Blush </i>", EndConversation(DialogueOutcome.Nice));
            }
        }

        private class McGrawPartyPositive : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("McGraw");
                DialogueSystem.Instance.WriteNPCLine("I was worried about you, but it worked out in the end.");
                DialogueSystem.Instance.WriteNPCLine("Q was hurting people, even if he doesn't see it that way.");
                DialogueSystem.Instance.WriteNPCLine("Thank you.");
                DialogueSystem.Instance.WritePlayerChoiceLine("In this case it was easy to do the right thing.", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("I'm still not sure I made the right decision.", EndConversation(DialogueOutcome.Mean));
            }
        }

        private class McGrawPartyNegative : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("McGraw");
                DialogueSystem.Instance.WriteNPCLine("Thanks to you Q is still pedling his horrible plants.");
                DialogueSystem.Instance.WriteNPCLine("If someone gets hurt it'll be on your head as well.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I don't know what you are talking about.", EndConversation(DialogueOutcome.Mean));
                DialogueSystem.Instance.WritePlayerChoiceLine("It's harmless and the crew loves it.", EndConversation(DialogueOutcome.Mean));
                DialogueSystem.Instance.WritePlayerChoiceLine("I do actually worry about that. Perhaps I can set it right.", EndConversation(DialogueOutcome.Nice));
            }
        }

        //Need to decide what happens with the romance story line.
        private class TolstoyPartyPositive : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Tolstoy");
                DialogueSystem.Instance.WriteNPCLine("Thanks to you things are looking up!");
                DialogueSystem.Instance.WriteNPCLine("I'm really glad you came on board.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Thanks, glad to be here.", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("Err, thanks. I guess.", EndConversation(DialogueOutcome.Default));
            }
        }

        private class TolstoyPartyNegative : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Tolstoy");
                DialogueSystem.Instance.WriteNPCLine("So then I said, Clam down Ellie, you gotta listen to me, Ellie.");
                DialogueSystem.Instance.WriteNPCLine("You know, nobody exists on purpose, nobody belongs anywhere, everybody's gunna die.");
                DialogueSystem.Instance.WriteNPCLine("She just walked off!");
                DialogueSystem.Instance.WritePlayerChoiceLine("Wow, that's pretty dark.", EndConversation(DialogueOutcome.Mean));
                DialogueSystem.Instance.WritePlayerChoiceLine("I know a drink that can help with that.", EndConversation(DialogueOutcome.Nice));
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
                DialogueSystem.Instance.WriteNPCLine("Poor Tolstoy's really lost it.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'll see if I can sort him a dirnk. That normally helps.", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("Some people will never be happy.", EndConversation(DialogueOutcome.Mean));
            }
        }
    }
}
