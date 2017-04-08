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
    static class StoryActions
    {
        #region PickyCustomer
        public static ActionSequence GettingFrosty(Entity main)
        {
            var sequence = new ActionSequence("GettingFrosty");
            sequence.Add(CommonActions.QueueForDrinkOrder(main, 0, 0));
            sequence.Add(new ConversationAction(new WeirdOrderDialogue()));
            sequence.Add(new DialogueBranchAction(new Dictionary<DialogueOutcome, Action>
            {
                    {DialogueOutcome.Bad, () => {
                        ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(main, new UpdateMoodAction(Mood.Angry));
                    }},
                    {DialogueOutcome.Default, () => {
                        ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(main, CommonActions.WaitForDrink(main, DrinkRecipes.GetDrinkRecipe("Mind Meld"), 10));
                    }}
            }));

            return sequence;
        }

        private class WeirdOrderDialogue : Conversation
        {
            protected override void StartConversation(string nameOfSpeaker)
            {
                DialogueSystem.Instance.StartDialogue(nameOfSpeaker);
                DialogueSystem.Instance.WriteNPCLine("Hey you. I want a Frosted Mind Meld.");
                DialogueSystem.Instance.WritePlayerChoiceLine("And I want to be treated like a sentient being.", SentientBeing);
                DialogueSystem.Instance.WritePlayerChoiceLine("Of Course", MakeItQuick);
                DialogueSystem.Instance.WritePlayerChoiceLine("‘Frosted’? Run that by me.", DrinkClarification);
            }

            private void SentientBeing()
            {
                DialogueSystem.Instance.NextPanel();
                DialogueSystem.Instance.WriteNPCLine("Umm. Please?");
                DialogueSystem.Instance.WritePlayerChoiceLine("Better, thanks. Now what was it?", DrinkClarification);
            }

            private void DrinkClarification()
            {
                DialogueSystem.Instance.NextPanel();
                DialogueSystem.Instance.WriteNPCLine("Mind Meld. Frosted. Don’t you have to take classes or something to sell this stuff?");
                DialogueSystem.Instance.WritePlayerChoiceLine("Listen buddy. Only thing getting frosted round here is your fracking head.", SpeakToSecurity);
                DialogueSystem.Instance.WritePlayerChoiceLine("Yeah, but most of the course were useless stuff like ‘Conflict Resolution 401’. Stupid, right?", ConflictResolution);
                DialogueSystem.Instance.WritePlayerChoiceLine("Ah, just didn’t hear you right the first time.", EndConversation(DialogueOutcome.Default));
            }

            private void SpeakToSecurity()
            {
                DialogueSystem.Instance.NextPanel();
                DialogueSystem.Instance.WriteNPCLine("I- I- I’m going to speak to security. I’m a big name. I’m important! You’ll see!");
                DialogueSystem.Instance.WritePlayerChoiceLine("...", EndConversation(DialogueOutcome.Bad));
            }

            private void ConflictResolution()
            {
                DialogueSystem.Instance.NextPanel();
                DialogueSystem.Instance.WriteNPCLine("Oh yeah. Tell me about it, all those dang classes.");
                DialogueSystem.Instance.WriteNPCLine("We have to do the same dumb stuff for Market Leadership.");
                DialogueSystem.Instance.WriteNPCLine("Can you beleive it? Anyhow, it’s just a Mind Meld with SpaceCola.");
                DialogueSystem.Instance.WriteNPCLine("Oh. But you’ve got to do they Synthol first.");
                DialogueSystem.Instance.WriteNPCLine("Then a drizzle of Cola. Over ice, of course. Then the Alk. Shaken. Drain the Synth.");
                DialogueSystem.Instance.WriteNPCLine("Then add fresh Synth.Then stick in the ‘fuge and spin out the Cola.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Uhuh. Sure.", EndConversation(DialogueOutcome.Default));
            }

            private void MakeItQuick()
            {
                DialogueSystem.Instance.NextPanel();
                DialogueSystem.Instance.WriteNPCLine("And make it quick.");
                DialogueSystem.Instance.WritePlayerChoiceLine("<i> sigh </i>", EndConversation(DialogueOutcome.Default));
            }
        }

        private class WeirdOrderDialogue2 : Conversation
        {
            protected override void StartConversation(string nameOfSpeaker)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region TolstoyRomantic
        public static void TolstoyRomantic(Entity main, Entity other, out ActionSequence mainActionSequence, out ActionSequence otherActionSequence)
        {
            mainActionSequence = new ActionSequence("TolstoyRomanticOneMain");
            otherActionSequence = new ActionSequence("TolstoyRomanticOneOther");

            var ordering = CommonActions.GoToPaypointAndOrderDrink(main, DrinkRecipes.GetRandomDrinkRecipe());            
            mainActionSequence.Add(ordering);

            var talkToPlayer = CommonActions.TalkToPlayer(new TolstoyOneDialogue());
            mainActionSequence.Add(talkToPlayer);

            otherActionSequence.Add(CommonActions.Wander());            

            var sync = new SyncedAction(main, other);
            mainActionSequence.Add(sync);
            otherActionSequence.Add(sync);

            mainActionSequence.Add(new SetTargetEntityAction(other));
            mainActionSequence.Add(new GoToMovingEntityAction());

            otherActionSequence.Add(new SetTargetEntityAction(main));
            otherActionSequence.Add(new GoToMovingEntityAction());

            var sync2 = new SyncedAction(main, other);
            mainActionSequence.Add(sync2);
            otherActionSequence.Add(sync2);

            var branchingDialogue = new DialogueBranchAction(
                new Dictionary<DialogueOutcome, Action>
                {
                    {DialogueOutcome.Romantic, () => {
                        ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(main, new UpdateMoodAction(Mood.Happy));
                    }},
                    {DialogueOutcome.Mean, () => {
                        ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(main, new UpdateMoodAction(Mood.Angry));
                    }},
                    {DialogueOutcome.Nice, () => {
                        ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(main, new UpdateMoodAction(Mood.Happy));
                    }}
            });

            mainActionSequence.Add(branchingDialogue);
            
            otherActionSequence.Add(new PauseAction(3));
            otherActionSequence.Add(new UpdateMoodAction(Mood.Angry));

            var sync3 = new SyncedAction(main, other);
            mainActionSequence.Add(sync3);
            otherActionSequence.Add(sync3);

            mainActionSequence.Add(CommonActions.TalkToPlayer(new TolstoyTwoDialogue()));
        }

        private class TolstoyOneDialogue : Conversation
        {
            protected override void StartConversation(string nameOfSpeaker)
            {
                DialogueSystem.Instance.StartDialogue(nameOfSpeaker);
                DialogueSystem.Instance.WriteNPCLine("I need some dating advice!");
                DialogueSystem.Instance.WritePlayerChoiceLine("Compliment her.", ComplimentHer);
                DialogueSystem.Instance.WritePlayerChoiceLine("Play hard to get.", PlayHardToGet);
                DialogueSystem.Instance.WritePlayerChoiceLine("Just be yourself.", BeYourself);
            }

            private void ComplimentHer()
            {
                DialogueSystem.Instance.NextPanel();
                DialogueSystem.Instance.WriteNPCLine("What should I say?! I never know what do tell women..");
                DialogueSystem.Instance.WritePlayerChoiceLine("You are the most beautiful woman in the galaxy.", Thanks);
                DialogueSystem.Instance.WritePlayerChoiceLine("I dream of a galaxy where your eyes are the stars and the universe worships the night.", Thanks);
                DialogueSystem.Instance.WritePlayerChoiceLine("You are the heart in my day and the soul in my night.", Thanks);                
            }

            private void Thanks()
            {
                DialogueSystem.Instance.NextPanel();
                DialogueSystem.Instance.WriteNPCLine("Wow! Thanks!");
                DialogueSystem.Instance.WritePlayerChoiceLine("Good luck.", EndConversation(DialogueOutcome.Romantic));
            }

            private void PlayHardToGet()
            {
                DialogueSystem.Instance.NextPanel();
                DialogueSystem.Instance.WriteNPCLine("Good idea! I read on the Spacenet that women love it when you ignore them.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Good luck.", EndConversation(DialogueOutcome.Mean));
            }

            private void BeYourself()
            {
                DialogueSystem.Instance.NextPanel();
                DialogueSystem.Instance.WriteNPCLine("That's what everyone tells me but it never seems to work... Well, here is goes.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Good luck.", EndConversation(DialogueOutcome.Nice));
            }
        }

        private class TolstoyTwoDialogue : Conversation
        {
            protected override void StartConversation(string nameOfSpeaker)
            {
                DialogueSystem.Instance.StartDialogue(nameOfSpeaker);
                DialogueSystem.Instance.WriteNPCLine("She said she had to take an emergency call but I didn't hear her communicator beep.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'm sure she had a great time.", EndConversation(DialogueOutcome.Default));
                DialogueSystem.Instance.WritePlayerChoiceLine("Better luck next time.", EndConversation(DialogueOutcome.Default));
                DialogueSystem.Instance.WritePlayerChoiceLine("Sorry, gotta wipe this up. Can't talk now.", EndConversation(DialogueOutcome.Default));
            }
        }
    }
    #endregion
}
