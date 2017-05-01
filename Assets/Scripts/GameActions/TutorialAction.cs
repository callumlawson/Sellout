using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.GameActions.Dialogue;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Dialogue;

namespace Assets.Scripts.GameActions
{
    static class TutorialAction
    {
        public static ActionSequence Tutorial(Entity tutorialGiver, Entity player)
        {
            var sequence = new ActionSequence("Tutorial");
            sequence.Add(CommonActions.TalkToPlayer(new TutorialDiaglogue()));
            sequence.Add(new DialogueBranchAction(new Dictionary<DialogueOutcome, Action>
            {
                {
                    DialogueOutcome.Nice, () =>
                    {
                        ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(tutorialGiver, new UpdateMoodAction(Mood.Happy));
                    }
                },
                {
                    DialogueOutcome.Mean, () =>
                    {
                         ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(tutorialGiver, new UpdateMoodAction(Mood.Angry));
                    }
                }
            }));
            //TODO custom drink ordering. 
            sequence.Add(CommonActions.GoToPaypointAndOrderDrink(tutorialGiver, DrinkRecipes.GetDrinkRecipe("Mind Meld"), 90));
            sequence.Add(new RemoveTutorialControlLockAction());
            sequence.Add(CommonActions.ShortSitDown(tutorialGiver));
            return sequence;
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
