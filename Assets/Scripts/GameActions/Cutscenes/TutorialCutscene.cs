﻿using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.GameActions.Dialogue;
using Assets.Scripts.GameActions.Waypoints;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Dialogue;
using Assets.Scripts.Util.NPC;

namespace Assets.Scripts.GameActions.Cutscenes
{
    static class TutorialCutscene
    {
        public static void Start(List<Entity> matchingEntities) {
           
            var mcGraw = EntityQueries.GetEntityWithName(matchingEntities, NPCS.McGraw.Name);
            var ellie = EntityQueries.GetEntityWithName(matchingEntities, NPCS.Ellie.Name);
            var tolstoy = EntityQueries.GetEntityWithName(matchingEntities, NPCS.Tolstoy.Name);
            var player = EntityQueries.GetEntityWithName(matchingEntities, "You");
            //var director = EntityQueries.GetEntityWithName(matchingEntities, "Director");

            var endOfTutorialSyncPoint = new SyncedAction(new List<Entity> { ellie, tolstoy} );

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
            orderSequence.Add(new StartDrinkOrderAction(new DrinkOrder
            {
                OrdererName = mcGraw.GetState<NameState>().Name,
                OrdererSpecies = "Human",
                Recipe = drinkRecipe
            }));
            mcGrawSequence.Add(orderSequence);
            orderSequence.Add(CommonActions.WaitForDrink(mcGraw, drinkRecipe, 90, true, new DrinkSucsessDialogue()));
            mcGrawSequence.Add(new RemoveTutorialControlLockAction());
            mcGrawSequence.Add(endOfTutorialSyncPoint);
            mcGrawSequence.Add(new TeleportAction(Locations.RandomSeatLocation()));
            mcGrawSequence.Add(CommonActions.SitDown());
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
            ellieSequence.Add(new TeleportAction(Locations.RandomSeatLocation()));
            ellieSequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(ellie, ellieSequence);

            //Tolstoy
            var tolstoySequence = new ActionSequence("Tolstoy morning");
            ellieSequence.Add(new PauseAction(0.5f)); //WORKAROUND FOR SYNC ACTION BUG
            tolstoySequence.Add(endOfTutorialSyncPoint);
            tolstoySequence.Add(new TeleportAction(Locations.RandomSeatLocation()));
            tolstoySequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(tolstoy, tolstoySequence);
        }

        private class TutorialIntroDiaglogue : Conversation
        {
            protected override void StartConversation(string nameOfSpeaker)
            {
                DialogueSystem.Instance.StartDialogue(nameOfSpeaker);
                DialogueSystem.Instance.WriteNPCLine("<i>Looks at you expectantly</i>");
                DialogueSystem.Instance.WriteNPCLine("Wait, you arn't Dave...");
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
            protected override void StartConversation(string nameOfSpeaker)
            {
                DialogueSystem.Instance.StartDialogue(nameOfSpeaker);
                DialogueSystem.Instance.WriteNPCLine("That's... actually pretty good.");
                DialogueSystem.Instance.WriteNPCLine("As you're new I'll give you some advice. Watch out for Q.");
                DialogueSystem.Instance.WriteNPCLine("Q and Dave got up to all sorts of trouble. Ship security were loitering here all the time.");
                DialogueSystem.Instance.WriteNPCLine("Now that you are here, perhaps we can have some peace and quiet!");
                DialogueSystem.Instance.WriteNPCLine("Till later!");
                DialogueSystem.Instance.WritePlayerChoiceLine("See you!", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("<i> Say nothing </i>", EndConversation(DialogueOutcome.Default));
            }
        }
    }
}
