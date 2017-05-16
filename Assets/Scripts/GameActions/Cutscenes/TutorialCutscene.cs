using System;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.GameActions.Decorators;
using Assets.Scripts.GameActions.Dialogue;
using Assets.Scripts.GameActions.Waypoints;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Dialogue;
using Assets.Scripts.Util.NPC;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.GameActions.Cutscenes
{
    static class TutorialCutscene
    {
        public static void Start(List<Entity> matchingEntities) {
           
            var mcGraw = EntityQueries.GetNPC(matchingEntities, NPCS.McGraw.Name);
            var player = EntityQueries.GetNPC(matchingEntities, "You");

            //TODO Timeouts are in game mins and don't work!
            var mcGrawSequence = new ActionSequence("McGrawTutorial");
            mcGrawSequence.Add(new TeleportAction(Locations.OutsideDoorLocation()));
            mcGrawSequence.Add(new GetWaypointAction(Goal.PayFor));
            mcGrawSequence.Add(new GoToWaypointAction());
            mcGrawSequence.Add(new ConversationAction(new TutorialDiaglogue()));
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
            orderSequence.Add(CommonActions.WaitForDrink(mcGraw, drinkRecipe, 90, true));

            mcGrawSequence.Add(orderSequence);
            mcGrawSequence.Add(new RemoveTutorialControlLockAction());
            mcGrawSequence.Add(CommonActions.ShortSitDown(mcGraw));

            var playerSequence = new ActionSequence("PlayerTutorial");
            playerSequence.Add(new TeleportAction(Locations.OutsideDoorLocation()));
            playerSequence.Add(new GetWaypointAction(Goal.RingUp));
            playerSequence.Add(new GoToWaypointAction());

            ActionManagerSystem.Instance.QueueAction(mcGraw, mcGrawSequence);
            ActionManagerSystem.Instance.QueueAction(player, playerSequence);
        }

        private class TutorialDiaglogue : Conversation
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
                DialogueSystem.Instance.WriteNPCLine("<i>Just click on the bar to get started...</i>");
            }
        }
    }
}
