using Assets.Framework.Entities;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using System;

namespace Assets.Scripts.Util.Dialogue
{
    public abstract class Conversation
    {
        private Action onEnd;
        private Entity entity;

        public void Start(Action onEnd, Entity entity)
        {
            this.onEnd = onEnd;
            this.entity = entity;

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

        protected Action EndConversation(DialogueOutcome outcome)
        {
            return () =>
            {
                entity.GetState<DialogueOutcomeState>().Outcome = outcome;
                onEnd();
                DialogueSystem.Instance.StopDialogue();
            };
        }
    }
}
