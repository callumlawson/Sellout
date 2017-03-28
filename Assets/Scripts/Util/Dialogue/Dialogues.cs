using Assets.Scripts.Systems;

namespace Assets.Scripts.Util.Dialogue
{
    public static class Dialogues
    {
        public static readonly PlayerInitiatedDialogueOne DialogueOne = new PlayerInitiatedDialogueOne();
        public static readonly PlayerInitiatedDialogueTwo DialogueTwo = new PlayerInitiatedDialogueTwo();
        public static readonly WrongDrinkConversation WrongDrinkDialogue = new WrongDrinkConversation();

        public class TellTheTimeConverstation : Conversation
        {
            private readonly string time;

            public TellTheTimeConverstation(string time)
            {
                this.time = time;
            }

            protected override void StartConversation(string nameOfSpeaker)
            {
                DialogueSystem.Instance.StartDialogue(nameOfSpeaker);
                DialogueSystem.Instance.WriteNPCLine("The time is: " + time);
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>Errrr, Thanks.</i>", EndConversation(DialogueOutcome.Nice));
            }
        }

        public class OrderDrinkConverstation : Conversation
        {
            private readonly string drinkName;

            public OrderDrinkConverstation(string drinkName)
            {
                this.drinkName = drinkName;
            }

            protected override void StartConversation(string nameOfSpeaker)
            {
                DialogueSystem.Instance.StartDialogue(nameOfSpeaker);
                DialogueSystem.Instance.WriteNPCLine("A " + drinkName + " please.");
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>Nod.</i>", EndConversation(DialogueOutcome.Default));
            }
        }

        public class WrongDrinkConversation : Conversation
        {
            protected override void StartConversation(string nameOfSpeaker)
            {
                DialogueSystem.Instance.StartDialogue(nameOfSpeaker);
                DialogueSystem.Instance.WriteNPCLine("That isn't what I ordered. <i> Throws drink into the sink! </i>");
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>Figures.</i>", EndConversation(DialogueOutcome.Default));
            }
        }

        public class PlayerInitiatedDialogueOne : Conversation
        {
            protected override void StartConversation(string nameOfSpeaker)
            {
                DialogueSystem.Instance.StartDialogue(nameOfSpeaker);
                DialogueSystem.Instance.WritePlayerDialogueLine("How you doing?");
                DialogueSystem.Instance.WriteNPCLine("Hmm, not too bad I guess. You?");
                DialogueSystem.Instance.WritePlayerChoiceLine("Huh. Nice of you to ask. Few do.", BitFriendly);
                DialogueSystem.Instance.WritePlayerChoiceLine("As of very recently I'm running the bar now.", RunningBar);
                DialogueSystem.Instance.WritePlayerChoiceLine("Fine, gotta wipe this up.", EndConversation(DialogueOutcome.Default));
            }

            private void BitFriendly()
            {
                DialogueSystem.Instance.WriteNPCLine("It's a small boat. Friendly will get you far.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Fair point - thanks.", EndConversation(DialogueOutcome.Default));
            }

            private void RunningBar()
            {
                DialogueSystem.Instance.WriteNPCLine("Ah, shame about poor Fred... still, glad you'll have the taps flowing again.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'll do my best.", EndConversation(DialogueOutcome.Default));
            }
        }

        public class PlayerInitiatedDialogueTwo : Conversation
        {
            protected override void StartConversation(string nameOfSpeaker)
            {
                DialogueSystem.Instance.StartDialogue(nameOfSpeaker);
                DialogueSystem.Instance.WritePlayerDialogueLine("Hey");
                DialogueSystem.Instance.WriteNPCLine("What you looking at?");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'm looking at you.", Whoops);
            }

            private void Whoops()
            {
                DialogueSystem.Instance.WriteNPCLine("What you looking at <b>me</b> for?");
                DialogueSystem.Instance.WritePlayerChoiceLine("I, err, don't know.", DiggingHole);
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>walk away</i>", EndConversation(DialogueOutcome.Default));
            }

            private void DiggingHole()
            {
                DialogueSystem.Instance.WriteNPCLine("Well sod off then.");
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>walk quickly away</i>", EndConversation(DialogueOutcome.Default));
            }
        }
    }
}
