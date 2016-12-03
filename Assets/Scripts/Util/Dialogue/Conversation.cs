using Assets.Scripts.Systems;
using System;

namespace Assets.Scripts.Util.Dialogue
{
    abstract class Conversation
    {
        private Action onEnd;

        public void Start(Action onEnd)
        {
            this.onEnd = onEnd;
            StartConversation();
        }

        protected abstract void StartConversation();        

        protected void EndConversation()
        {
            onEnd();
            DialogueSystem.Instance.StopDialogue();
        }
    }
}
