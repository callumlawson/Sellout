using Assets.Scripts.Systems;
using System;

namespace Assets.Scripts.Util.Dialogue
{
    public abstract class Conversation
    {
        private Action onEnd;

        public void Start(Action onEnd)
        {
            this.onEnd = onEnd;
            StartConversation();
        }

        public void Pause()
        {
            DialogueSystem.Instance.PauseDialogue();
        }

        public void Unpause()
        {
            DialogueSystem.Instance.UnpauseDialogue();
        }

        protected abstract void StartConversation();      

        protected void EndConversation()
        {
            onEnd();
            DialogueSystem.Instance.StopDialogue();
        }
    }
}
