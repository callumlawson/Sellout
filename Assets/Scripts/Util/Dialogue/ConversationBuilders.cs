using Assets.Scripts.Systems;

namespace Assets.Scripts.Util.Dialogue
{
    public class SingleOutcomeConversation : Conversation
    {
        private string[] lines;
        private string response;
        private DialogueOutcome outcome;

        public SingleOutcomeConversation(string line, string response, DialogueOutcome outcome)
        {
            this.lines = new string[] { line };
            this.response = response;
            this.outcome = outcome;
        }

        public SingleOutcomeConversation(string[] lines, string response, DialogueOutcome outcome)
        {
            this.lines = lines;
            this.response = response;
            this.outcome = outcome;
        }

        protected override void StartConversation(string converstationInitiator)
        {
            DialogueSystem.Instance.StartDialogue(converstationInitiator);
            foreach (var line in lines)
            {
                DialogueSystem.Instance.WriteNPCLine(line);
            }
            DialogueSystem.Instance.WritePlayerChoiceLine("<i>" + response + "</i>", EndConversation(outcome));
        }
    }

    public class WaitConversation : Conversation
    {
        private string[] lines;

        public WaitConversation(string line)
        {
            this.lines = new string[] { line };
        }

        public WaitConversation(string[] lines)
        {
            this.lines = lines;
        }

        protected override void StartConversation(string converstationInitiator)
        {
            DialogueSystem.Instance.StartDialogue(converstationInitiator);
            foreach (var line in lines)
            {
                DialogueSystem.Instance.WriteNPCLine(line);
            }
        }
    }

    public class NoResponseConversation : Conversation
    {
        private string[] lines;
        private DialogueOutcome outcome;

        public NoResponseConversation(string line, DialogueOutcome outcome)
        {
            this.lines = new string[] { line };
            this.outcome = outcome;
        }

        public NoResponseConversation(string[] lines, DialogueOutcome outcome)
        {
            this.lines = lines;
            this.outcome = outcome;
        }

        protected override void StartConversation(string converstationInitiator)
        {
            DialogueSystem.Instance.StartDialogue(converstationInitiator, 1.5f, EndConversation(outcome));
            foreach (var line in lines)
            {
                DialogueSystem.Instance.WriteNPCLine(line);
            }
        }
    }

}

