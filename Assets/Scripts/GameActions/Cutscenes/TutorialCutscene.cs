using System;
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
           
            var mcGraw = EntityQueries.GetNPC(matchingEntities, NPCS.McGraw.Name);
            var player = EntityQueries.GetNPC(matchingEntities, "You");
            //var camera = todo

            var mcGrawSequence = new ActionSequence("McGrawTutorial");
            mcGrawSequence.Add(new TeleportAction(Locations.OutsideDoorLocation()));
            mcGrawSequence.Add(new GetWaypointAction(Goal.PayFor));
            mcGrawSequence.Add(new GoToWaypointAction());
            mcGrawSequence.Add(new ConversationAction(new TutorialDiaglogue()));
            mcGrawSequence.Add(new DialogueBranchAction(new Dictionary<DialogueOutcome, Action>
            {
                {
                    DialogueOutcome.Nice, () =>
                    {
                        ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(mcGraw, new UpdateMoodAction(Mood.Happy));
                    }
                },
                {
                    DialogueOutcome.Mean, () =>
                    {
                         ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(mcGraw, new UpdateMoodAction(Mood.Angry));
                    }
                }
            }));
            //TODO custom drink ordering. 
            mcGrawSequence.Add(CommonActions.GoToPaypointAndOrderDrink(mcGraw, DrinkRecipes.GetDrinkRecipe("Mind Meld"), 90));
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
                DialogueSystem.Instance.WritePlayerChoiceLine("I am indeed not Dave.", SoItBegings);
            }

            private void SoItBegings()
            {
                DialogueSystem.Instance.NextPanel();
                DialogueSystem.Instance.WriteNPCLine("Ahhhr, now I'm going to have to train you up as well.");
                DialogueSystem.Instance.WriteNPCLine("Don't worry, my tipple is easy.");
                DialogueSystem.Instance.WriteNPCLine("Can't get the brain going without a morning Mind Meld.");
                DialogueSystem.Instance.WriteNPCLine("Just grab that glass and stab it at the Synthol dispenser thrice. Then a single measure of Alcohol.");
                DialogueSystem.Instance.WritePlayerChoiceLine("You drink alcohol straight. For breakfast?", Problem);
            }

            private void Problem()
            {
                DialogueSystem.Instance.NextPanel();
                DialogueSystem.Instance.WriteNPCLine("Problem with that?");
                DialogueSystem.Instance.WritePlayerChoiceLine("I guess not. Err, coming right up", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("It's your liver.", EndConversation(DialogueOutcome.Mean));
            }
        }
    }
}
