using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util.Dialogue;

namespace Assets.Scripts.GameActions
{
    class SetConversationAction : GameAction, ICancellableAction 
    {
        private readonly Conversation conversation;

        private readonly Dictionary<DialogueOutcome, Action> defaultReactions; 

        public SetConversationAction(Conversation conversation)
        {
            this.conversation = conversation;
        }

        public SetConversationAction(Conversation conversation, Entity reactingEntity)
        {
            this.conversation = conversation;

            defaultReactions = new Dictionary<DialogueOutcome, Action>
                {
                    {
                        DialogueOutcome.Nice, () => ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(reactingEntity, new UpdateMoodAction(Mood.Happy))
                    },
                    {
                        DialogueOutcome.Mean, () => ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(reactingEntity, new UpdateMoodAction(Mood.Angry))
                    }
                };

            conversation.OnDialoguOutcome += OnConverstationFinished;
        }

        private void OnConverstationFinished(DialogueOutcome outcome)
        {
            if (defaultReactions.ContainsKey(outcome))
            {
                defaultReactions[outcome].Invoke();
            }
        }

        public override void OnFrame(Entity entity)
        {
            //Do Nothing
        }

        public override void OnStart(Entity entity)
        {
            entity.GetState<ConversationState>().Conversation = conversation;
            ActionStatus = ActionStatus.Succeeded;
        }

        public override void Pause()
        {
            //Do Nothing
        }

        public override void Unpause()
        {
            //Do Nothing
        }

        public void Cancel()
        {
            //Do Nothing
        }

        public bool IsCancellable()
        {
            return true;
        }
    }
}
