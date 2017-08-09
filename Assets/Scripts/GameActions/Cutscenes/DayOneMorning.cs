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
using Assets.Scripts.GameActions.Stories;

namespace Assets.Scripts.GameActions.Cutscenes
{
    static class DayOneMorning
    {
        public static void Start(List<Entity> matchingEntities) {
           
            var mcGraw = EntityQueries.GetEntityWithName(matchingEntities, NPCS.McGraw.Name);
            var player = EntityQueries.GetEntityWithName(matchingEntities, NPCName.You);

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
                mcGrawSequence.Add(new FadeToBlackAction(6.5f, "First day. Just open the bar then serve the right drinks, Easy?"));
                mcGrawSequence.Add(new PauseAction(3.0f));
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
                mcGrawSequence.Add(new TeleportAction(Locations.SitDownPoint3()));
                mcGrawSequence.Add(new SetReactiveConversationAction(new McGrawMorningOne()));
                mcGrawSequence.Add(CommonActions.SitDownLoop());
                ActionManagerSystem.Instance.QueueAction(mcGraw, mcGrawSequence);
            }

            var loveStoryActions = LoveStory.DayOneMorning();
            foreach (var actionPair in loveStoryActions)
            {
                var entity = actionPair.GetEntity();
                var action = actionPair.GetGameAction();
                ActionManagerSystem.Instance.QueueAction(entity, action);
            }
        }

        private class TutorialIntroDiaglogue : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("Morning!");
                DialogueSystem.Instance.WriteNPCLine("I should introduce myself. McGraw - head of security");
                DialogueSystem.Instance.WritePlayerChoiceLine("Thanks, I'm still learning names. What can I do for you at this early hour?", SoItBegings);
            }

            private void SoItBegings()
            {
                DialogueSystem.Instance.NextPanel();
                DialogueSystem.Instance.WriteNPCLine("Don't worry, my favorite drink - a Mind Meld - is easy to make.");
                DialogueSystem.Instance.WriteNPCLine("I can't get my brain going without it.");
                DialogueSystem.Instance.WriteNPCLine("You can find the recipe by touching the blue 'Drinks' panel to your left.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I know how to make a Mind Meld, thanks...", EndConversation(DialogueOutcome.Mean));
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
                DialogueSystem.Instance.WriteNPCLine("As you're new I'll give you some advice...");
                DialogueSystem.Instance.WriteNPCLine("Watch out for Q, he gets up to all sorts of trouble and I don't want that with you.");
                DialogueSystem.Instance.WriteNPCLine("Once you have introduced yourself to the crew you need to activate the console near the door.");
                DialogueSystem.Instance.WriteNPCLine("That will let them know it's time for drinks!");
                DialogueSystem.Instance.WriteNPCLine("See you around!");
                DialogueSystem.Instance.WritePlayerChoiceLine("Until next time.", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>Say nothing</i>", EndConversation(DialogueOutcome.Default));
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
