using Assets.Framework.Entities;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using System;

namespace Assets.Scripts.Util.Dialogue
{
    [Serializable]
    public abstract class Conversation
    {
        private Action onEnd;
        private Entity entity;

        public void Start(Action onEndAction, Entity conversingEntity)
        {
            onEnd = onEndAction;
            entity = conversingEntity;
            StartConversation(entity.GetState<NameState>().Name);
        }

        public void Pause()
        {
            DialogueSystem.Instance.PauseDialogue();
        }

        public void Unpause()
        {
            DialogueSystem.Instance.UnpauseDialogue();
        }

        protected abstract void StartConversation(string nameOfSpeaker);      

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
