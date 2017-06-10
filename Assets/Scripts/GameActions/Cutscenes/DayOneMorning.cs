using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
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
            var player = EntityQueries.GetEntityWithName(matchingEntities, "You");

            var endOfTutorialSyncPoint = new SyncedAction(new List<Entity> { mcGraw, ellie, tolstoy} );

            //McGraw
            var mcGrawSequence = new ActionSequence("McGrawTutorial");
            mcGrawSequence.Add(new TeleportAction(Locations.OutsideDoorLocation()));
            mcGrawSequence.Add(new GetWaypointAction(Goal.PayFor));
            mcGrawSequence.Add(new GoToWaypointAction());
            mcGrawSequence.Add(new ConversationAction(new TutorialIntroDiaglogue()));
            mcGrawSequence.Add(new DialogueBranchAction(new Dictionary<DialogueOutcome, Action>
            {
                {
                    DialogueOutcome.Nice, () => ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(mcGraw, new UpdateMoodAction(Mood.Happy))
                },
                {
                    DialogueOutcome.Mean, () => ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(mcGraw, new UpdateMoodAction(Mood.Angry))
                }
            }));
            mcGrawSequence.Add(CommonActions.QueueForDrinkOrder(mcGraw, 10, 20));
            const string drinkName = "Mind Meld";
            var drinkRecipe = DrinkRecipes.GetDrinkRecipe(drinkName);

            var orderSequence = new ConditionalActionSequence("Drink order", false);
            var drinkOrder = new DrinkOrders.ExactDrinkorder(drinkRecipe, mcGraw.GetState<NameState>().Name);
            orderSequence.Add(new StartDrinkOrderAction(drinkOrder));
            mcGrawSequence.Add(orderSequence);
            orderSequence.Add(CommonActions.WaitForDrink(mcGraw, drinkOrder.DrinkPredicate, 90, true, new DrinkSucsessDialogue()));
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
            mcGrawSequence.Add(new SetConversationAction(new McGrawMorningOne()));
            mcGrawSequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(mcGraw, mcGrawSequence);

            //Player
            var playerSequence = new ActionSequence("PlayerTutorial");
            playerSequence.Add(new TeleportAction(Locations.OutsideDoorLocation()));
            playerSequence.Add(new GetWaypointAction(Goal.RingUp));
            playerSequence.Add(new GoToWaypointAction());
            ActionManagerSystem.Instance.QueueAction(player, playerSequence);

            //Ellie
            var ellieSequence = new ActionSequence("Ellie morning");
            ellieSequence.Add(new PauseAction(0.5f)); //WORKAROUND FOR SYNC ACTION BUG
            ellieSequence.Add(endOfTutorialSyncPoint);
            ellieSequence.Add(new TeleportAction(Locations.SitDownPoint1()));
            ellieSequence.Add(new SetConversationAction(new EllieMorningOne()));
            ellieSequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(ellie, ellieSequence);

            //Tolstoy
            var tolstoySequence = new ActionSequence("Tolstoy morning");
            tolstoySequence.Add(new PauseAction(0.5f)); //WORKAROUND FOR SYNC ACTION BUG
            tolstoySequence.Add(endOfTutorialSyncPoint);
            tolstoySequence.Add(new TeleportAction(Locations.SitDownPoint2()));
            tolstoySequence.Add(new SetConversationAction(new TolstoyMorningOne()));
            tolstoySequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(tolstoy, tolstoySequence);
        }

        private class TutorialIntroDiaglogue : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("<i>Looks at you expectantly</i>");
                DialogueSystem.Instance.WriteNPCLine("Wait, you aren't Dave...");
                DialogueSystem.Instance.WritePlayerChoiceLine("Nope, I'm new. Poor Dave had to jump ship. What can I get you this morning?", SoItBegings);
            }

            private void SoItBegings()
            {
                DialogueSystem.Instance.NextPanel();
                DialogueSystem.Instance.WriteNPCLine("Sigh, now I'm going to have to train you up as well.");
                DialogueSystem.Instance.WriteNPCLine("Don't worry, my favorite drink is easy to make.");
                DialogueSystem.Instance.WriteNPCLine("Just grab that glass and dispense three measures of Synthol then a single measure of Alcohol.");
                DialogueSystem.Instance.WriteNPCLine("Can't get the brain going without a morning Mind Meld!");
                DialogueSystem.Instance.WritePlayerChoiceLine("You drink alcohol straight. For breakfast?", Problem);
            }

            private void Problem()
            {
                DialogueSystem.Instance.NextPanel();
                DialogueSystem.Instance.WriteNPCLine("Problem with that?");
                DialogueSystem.Instance.WritePlayerChoiceLine("I guess not. Coming right up", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("It's your liver.", EndConversation(DialogueOutcome.Mean));
                DialogueSystem.Instance.WriteNPCLine("<i>Click on the bar to get started...</i>");
            }
        }

        private class DrinkSucsessDialogue : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("That's... actually pretty good.");
                DialogueSystem.Instance.WriteNPCLine("As you're new I'll give you some advice. Watch out for Q.");
                DialogueSystem.Instance.WriteNPCLine("Q and Dave got up to all sorts of trouble. Ship security were loitering here all the time.");
                DialogueSystem.Instance.WriteNPCLine("Now that you are here, perhaps we can have some peace and quiet!");
                DialogueSystem.Instance.WriteNPCLine("Till later!");
                DialogueSystem.Instance.WritePlayerChoiceLine("See you!", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>Say nothing</i>", EndConversation(DialogueOutcome.Default));
            }
        }

        private class TolstoyMorningOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Tolstoy");
                DialogueSystem.Instance.WriteNPCLine("Hey, how you doing? Fine? Good.");
                DialogueSystem.Instance.WriteNPCLine("Don't you think Ellie's great?");
                DialogueSystem.Instance.WriteNPCLine("You should talk to her.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Err, sure. I want to meet everyone.", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("Perhaps some other time. Got to get the bar ready!", EndConversation(DialogueOutcome.Default));
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
                DialogueSystem.Instance.WriteNPCLine("When it's time to open the bar you need to let the crew know.");
                DialogueSystem.Instance.WriteNPCLine("Use the blue panel to my right inform the ship's computer you are ready.");
                DialogueSystem.Instance.WriteNPCLine("Good luck!");
                DialogueSystem.Instance.WritePlayerChoiceLine("Thanks.", EndConversation(DialogueOutcome.Nice));
            }
        }
    }
}
