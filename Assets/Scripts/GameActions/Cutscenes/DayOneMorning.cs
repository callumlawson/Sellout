using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.GameActions.Dialogue;
using Assets.Scripts.GameActions.Drinks;
using Assets.Scripts.GameActions.Inventory;
using Assets.Scripts.GameActions.Waypoints;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Dialogue;
using Assets.Scripts.Util.NPC;

namespace Assets.Scripts.GameActions.Cutscenes
{
    static class DayOneMorning
    {
        public static void Start(List<Entity> matchingEntities) {
           
            var mcGraw = EntityQueries.GetEntityWithName(matchingEntities, NPCS.McGraw.Name);
            var ellie = EntityQueries.GetEntityWithName(matchingEntities, NPCS.Ellie.Name);
            var tolstoy = EntityQueries.GetEntityWithName(matchingEntities, NPCS.Tolstoy.Name);
            var player = EntityQueries.GetEntityWithName(matchingEntities, NPCName.You);

            var endOfTutorialSyncPoint = new SyncedAction(new List<Entity> { mcGraw, ellie, tolstoy} );

            //McGraw
            var mcGrawSequence = new ActionSequence("McGrawTutorial");

            if (!GameSettings.DisableTutorial)
            {
                mcGrawSequence.Add(new TeleportAction(Locations.OutsideDoorLocation()));
                mcGrawSequence.Add(new GetWaypointAction(Goal.PayFor));
                mcGrawSequence.Add(new GoToWaypointAction());
                mcGrawSequence.Add(new ConversationAction(new TutorialIntroDiaglogue()));
                mcGrawSequence.Add(new DialogueBranchAction(new Dictionary<DialogueOutcome, Action>
                {
                    {
                        DialogueOutcome.Nice, () =>
                        {
                            ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(mcGraw,
                                new UpdateMoodAction(Mood.Happy));
                            StaticStates.Get<OutcomeTrackerState>().AddOutcome("You made a good impression on McGraw.");
                        }
                    },
                    {
                        DialogueOutcome.Mean, () =>
                        {
                            ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(mcGraw,
                                new UpdateMoodAction(Mood.Angry));
                            StaticStates.Get<OutcomeTrackerState>().AddOutcome("You and McGraw haven't got off to the best start.");
                        }
                    }
                }));
                mcGrawSequence.Add(CommonActions.QueueForDrinkOrder(mcGraw, 10, 20));
                const string drinkName = "Mind Meld";
                var drinkRecipe = DrinkRecipes.GetDrinkRecipe(drinkName);

                var orderSequence = new ConditionalActionSequence("Drink order", false);
                var drinkOrder = new DrinkOrders.ExactDrinkorder(drinkRecipe, mcGraw.GetState<NameState>().Name);
                orderSequence.Add(new StartDrinkOrderAction(drinkOrder));
                mcGrawSequence.Add(orderSequence);
                orderSequence.Add(CommonActions.WaitForDrink(mcGraw, drinkName, drinkOrder, 90, true, new DrinkSucsessDialogue()));
                mcGrawSequence.Add(new RemoveTutorialControlLockAction());
                mcGrawSequence.Add(new FadeToBlackAction(6.5f, "Alright, First day. Just open the bar then serve the right drinks. Easy."));
                mcGrawSequence.Add(new PauseAction(3.0f));
                mcGrawSequence.Add(endOfTutorialSyncPoint);
                mcGrawSequence.Add(new CallbackAction(() =>
                {
                    EventSystem.EndDrinkMakingEvent.Invoke();
                    ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(player,
                        new TeleportAction(Locations.CenterOfBar()));
                })); //This is kind of dirty - but demo!
                mcGrawSequence.Add(new DestoryEntityInInventoryAction());
                mcGrawSequence.Add(new TeleportAction(Locations.SitDownPoint3()));
                mcGrawSequence.Add(new SetReactiveConversationAction(new McGrawMorningOne(), mcGraw));
                mcGrawSequence.Add(CommonActions.SitDownLoop());
                ActionManagerSystem.Instance.QueueAction(mcGraw, mcGrawSequence);

                //Player
                var playerSequence = new ActionSequence("PlayerTutorial");
                playerSequence.Add(new TeleportAction(Locations.OutsideDoorLocation()));
                playerSequence.Add(new GetWaypointAction(Goal.RingUp));
                playerSequence.Add(new GoToWaypointAction());
                ActionManagerSystem.Instance.QueueAction(player, playerSequence);
            }
            else
            {
                mcGrawSequence.Add(endOfTutorialSyncPoint);
                mcGrawSequence.Add(new TeleportAction(Locations.SitDownPoint3()));
                mcGrawSequence.Add(new SetReactiveConversationAction(new McGrawMorningOne()));
                mcGrawSequence.Add(CommonActions.SitDownLoop());
                ActionManagerSystem.Instance.QueueAction(mcGraw, mcGrawSequence);
            }

            //Ellie
            var ellieSequence = new ActionSequence("Ellie morning");
            ellieSequence.Add(new PauseAction(0.5f)); //WORKAROUND FOR SYNC ACTION BUG
            ellieSequence.Add(endOfTutorialSyncPoint);
            ellieSequence.Add(new TeleportAction(Locations.SitDownPoint1()));
            ellieSequence.Add(new SetReactiveConversationAction(new EllieMorningOne(), ellie));
            ellieSequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(ellie, ellieSequence);

            //Tolstoy
            var tolstoySequence = new ActionSequence("Tolstoy morning");
            tolstoySequence.Add(new PauseAction(0.5f)); //WORKAROUND FOR SYNC ACTION BUG
            tolstoySequence.Add(endOfTutorialSyncPoint);
            tolstoySequence.Add(new TeleportAction(Locations.SitDownPoint2()));
            tolstoySequence.Add(new SetReactiveConversationAction(new TolstoyMorningOne(), tolstoy));
            tolstoySequence.Add(new TriggerAnimationAction(AnimationEvent.SittingStartTrigger));
            tolstoySequence.Add(CommonActions.WaitForDrink(tolstoy, "None", new DrinkOrders.AlwaysSucceedsDrinkOrder(), 99999));
            tolstoySequence.Add(new UpdateMoodAction(Mood.Happy));
            tolstoySequence.Add(new ConversationAction(new TolstoyMorningGivenDrink()));
            tolstoySequence.Add(new SetReactiveConversationAction(new TolstoyMorningAfterDrink()));
            tolstoySequence.Add(new CallbackAction(() => StaticStates.Get<PlayerDecisionsState>().GaveTolstoyDrink = true));
            tolstoySequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(tolstoy, tolstoySequence);
        }

        private class TutorialIntroDiaglogue : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("Morning!");
                DialogueSystem.Instance.WriteNPCLine("I should introduce myself. McGraw - head of security");
                DialogueSystem.Instance.WritePlayerChoiceLine("Thanks, I'm still learning names. What can I get you at this early hour?", SoItBegings);
            }

            private void SoItBegings()
            {
                DialogueSystem.Instance.NextPanel();
                DialogueSystem.Instance.WriteNPCLine("Don't worry, my favorite drink is easy to make.");
                DialogueSystem.Instance.WriteNPCLine("Grab a glass from the dispenser on your left and pour me three measures of Synthol and a single measure of Vodka.");
                DialogueSystem.Instance.WriteNPCLine("I can't get my brain going without it.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I know where to find a glass on my own bar...", EndConversation(DialogueOutcome.Mean));
                DialogueSystem.Instance.WritePlayerChoiceLine("You drink a glass of basically straight alcohol for breakfast?", Problem);
                DialogueSystem.Instance.WritePlayerChoiceLine("Coming right up!", EndConversation(DialogueOutcome.Nice));
            }

            private void Problem()
            {
                DialogueSystem.Instance.NextPanel();
                DialogueSystem.Instance.WriteNPCLine("Problem with that?");
                DialogueSystem.Instance.WritePlayerChoiceLine("I guess not. Coming right up.", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("It's your liver.", EndConversation(DialogueOutcome.Mean));
            }
        }

        private class DrinkSucsessDialogue : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("That's... actually pretty good.");
                DialogueSystem.Instance.WriteNPCLine("As you're new I'll give you some advice. Watch out for Q.");
                DialogueSystem.Instance.WriteNPCLine("He and Dave got up to all sorts of trouble and I don't want that with you.");
                DialogueSystem.Instance.WriteNPCLine("Now that you are here, perhaps I can have some peace and quiet!");
                DialogueSystem.Instance.WriteNPCLine("To open the bar you need to let the crew know.");
                DialogueSystem.Instance.WriteNPCLine("The coms console is on the right of the door.");
                DialogueSystem.Instance.WriteNPCLine("See you around!");
                DialogueSystem.Instance.WritePlayerChoiceLine("Until next time.", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>Say nothing</i>", EndConversation(DialogueOutcome.Default));
            }
        }

        private class TolstoyMorningOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(NPCName.Tolstoy.ToString());
                DialogueSystem.Instance.WriteNPCLine("Hey, how you doing? Fine? Good.");
                DialogueSystem.Instance.WriteNPCLine("Don't you think Ellie's great?");
                DialogueSystem.Instance.WriteNPCLine("You should talk to her.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Err, sure. I want to meet everyone.", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("Perhaps some other time. Got to get the bar ready!", EndConversation(DialogueOutcome.Default));
            }
        }

        private class TolstoyMorningGivenDrink : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(NPCName.Tolstoy.ToString());
                DialogueSystem.Instance.WriteNPCLine("Wow, thanks. I really needed this.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Everyone has one of those days occasionally.", EndConversation(DialogueOutcome.Default));
            }
        }

        private class TolstoyMorningAfterDrink : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(NPCName.Tolstoy.ToString());
                DialogueSystem.Instance.WriteNPCLine("Really, thank you!");
                DialogueSystem.Instance.WritePlayerChoiceLine("No worries", EndConversation(DialogueOutcome.Default));
            }
        }

        private class EllieMorningOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Ellie");
                DialogueSystem.Instance.WriteNPCLine("Hi. Good luck with your new post. I'm sure you'll do fine.");
                DialogueSystem.Instance.WriteNPCLine("Hmm, Tolstoy really looks like he needs a drink.");
                DialogueSystem.Instance.WriteNPCLine("He's been all wierd and agitated recently.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Luckily serving drinks is my forte!", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("Sure.", EndConversation(DialogueOutcome.Mean));
            }
        }

        private class McGrawMorningOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("McGraw");
                DialogueSystem.Instance.WriteNPCLine("Don't forget, to open the bar you need to let the crew know.");
                DialogueSystem.Instance.WriteNPCLine("Use the blue panel to my right.");
                DialogueSystem.Instance.WriteNPCLine("Good luck!");
                DialogueSystem.Instance.WritePlayerChoiceLine("Thanks.", EndConversation(DialogueOutcome.Default));
            }
        }
    }
}
