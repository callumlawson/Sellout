using Assets.Scripts.Systems;

namespace Assets.Scripts.Util.Dialogue
{
    public static class Dialogues
    {
        public static readonly DemoDialogueOne DialogueOne = new DemoDialogueOne();
        public static readonly DemoDialogueTwo DialogueTwo = new DemoDialogueTwo();
        public static readonly OrderDrinkConverstation OrderDrinkDialogue = new OrderDrinkConverstation();
        public static readonly WrongDrinkConversation WrongDrinkDialogue = new WrongDrinkConversation();

        public class OrderDrinkConverstation : Conversation
        {
            protected override void StartConversation()
            {
                DialogueSystem.Instance.StartDialogue();
                DialogueSystem.Instance.WriteNPCLine("Once Space Screwdriver please.");
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>Nod.</i>", EndConversation);
            }
        }

        public class WrongDrinkConversation : Conversation
        {
            protected override void StartConversation()
            {
                DialogueSystem.Instance.StartDialogue();
                DialogueSystem.Instance.WriteNPCLine("That isn't what I ordered. I'll drink it. I won't pay for it.");
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>Figures.</i>", EndConversation);
            }
        }

        public class DemoDialogueOne : Conversation
        {
            protected override void StartConversation()
            {
                DialogueSystem.Instance.StartDialogue();
                DialogueSystem.Instance.WriteNPCLine("Hello there, what you up to?");
                DialogueSystem.Instance.WritePlayerChoiceLine("You're a bit friendly.", BitFriendly);
                DialogueSystem.Instance.WritePlayerChoiceLine("I'm running the bar now.", RunningBar);
                DialogueSystem.Instance.WritePlayerChoiceLine("Sorry, gotta wipe this up. Can't talk now.", EndConversation);
            }

            private void BitFriendly()
            {
                DialogueSystem.Instance.WriteNPCLine("It's a small boat. Friendly will get you far.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Fair point - thanks.", EndConversation);
            }

            private void RunningBar()
            {
                DialogueSystem.Instance.WriteNPCLine("Ah, shame about poor Fred... still, glad you'll have the taps flowing again.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'll do my best.", EndConversation);
            }
        }

        public class DemoDialogueTwo : Conversation
        {
            protected override void StartConversation()
            {
                DialogueSystem.Instance.StartDialogue();
                DialogueSystem.Instance.WriteNPCLine("What you looking at?");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'm looking at you.", Whoops);
            }

            private void Whoops()
            {
                DialogueSystem.Instance.WriteNPCLine("What you looking at <b>me</b> for?");
                DialogueSystem.Instance.WritePlayerChoiceLine("I, err, don't know.", DiggingHole);
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>walk away</i>", EndConversation);
            }

            private void DiggingHole()
            {
                DialogueSystem.Instance.WriteNPCLine("Well sod off then.");
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>walk quickly away</i>", EndConversation);
            }
        }
    }
}
