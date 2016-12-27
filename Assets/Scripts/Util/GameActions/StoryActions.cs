using Assets.Framework.Entities;
using Assets.Scripts.GameActions;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.Systems;
using Assets.Scripts.Util.Dialogue;

namespace Assets.Scripts.Util.GameActions
{
    class StoryActions
    {
        public static void TolstoyRomantic(Entity main, Entity other, out ActionSequence mainActionSequence, out ActionSequence otherActionSequence)
        {
            mainActionSequence = new ActionSequence("TolstoyRomanticOneMain");
            otherActionSequence = new ActionSequence("TolstoyRomanticOneOther");

            var ordering = CommonActions.OrderDrink(main, DrinkRecipes.GetRandomDrinkRecipe());
            var talkToPlayer = CommonActions.TalkToPlayer(new TolstoyOneDialogue());            
            mainActionSequence.Add(ordering);
            mainActionSequence.Add(talkToPlayer);
                        
            otherActionSequence.Add(CommonActions.Wander());

            var sync = new SyncedAction(main, other);
            mainActionSequence.Add(sync);
            otherActionSequence.Add(sync);

            mainActionSequence.Add(new PauseAction(10));
            otherActionSequence.Add(new PauseAction(10));

            var sync2 = new SyncedAction(main, other);
            mainActionSequence.Add(sync2);
            otherActionSequence.Add(sync2);

            mainActionSequence.Add(CommonActions.TalkToPlayer(new TolstoyTwoDialogue()));
        }

        private class TolstoyOneDialogue : Conversation
        {
            protected override void StartConversation()
            {
                DialogueSystem.Instance.StartDialogue();
                DialogueSystem.Instance.WriteNPCLine("I need your advice.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Compliment her.", ComplimentHer);
                DialogueSystem.Instance.WritePlayerChoiceLine("Play hard to get.", PlayHardToGet);
                DialogueSystem.Instance.WritePlayerChoiceLine("Just be yourself.", BeYourself);
            }

            private void ComplimentHer()
            {
                DialogueSystem.Instance.WriteNPCLine("What should I say?! I never know what do tell women..");
                DialogueSystem.Instance.WritePlayerChoiceLine("You are the most beautiful woman in the galaxy.", Thanks);
                DialogueSystem.Instance.WritePlayerChoiceLine("I dream of a galaxy where your eyes are the stars and the universe worships the night.", Thanks);
                DialogueSystem.Instance.WritePlayerChoiceLine("You are the heart in my day and the soul in my night.", Thanks);                
            }

            private void Thanks()
            {
                DialogueSystem.Instance.WriteNPCLine("Wow! Thanks!");
                DialogueSystem.Instance.WritePlayerChoiceLine("Good luck.", EndConversation);
            }

            private void PlayHardToGet()
            {
                DialogueSystem.Instance.WriteNPCLine("Good idea! I read on the Spacenet that women love it when you ignore them.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Good luck.", EndConversation);
            }

            private void BeYourself()
            {
                DialogueSystem.Instance.WriteNPCLine("That's what everyone tells me but it never seems to work... Well, here is goes.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Good luck.", EndConversation);
            }
        }

        private class TolstoyTwoDialogue : Conversation
        {
            protected override void StartConversation()
            {
                DialogueSystem.Instance.StartDialogue();
                DialogueSystem.Instance.WriteNPCLine("She said she had to take an emergency call but I didn't hear her communicator beep.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'm sure she had a great time.", EndConversation);
                DialogueSystem.Instance.WritePlayerChoiceLine("Better luck next time.", EndConversation);
                DialogueSystem.Instance.WritePlayerChoiceLine("Sorry, gotta wipe this up. Can't talk now.", EndConversation);
            }
        }
    }
}
