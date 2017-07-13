using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util.Dialogue;

namespace Assets.Scripts.GameActions
{
    class SetReactiveConversationAction : GameAction, ICancellableAction 
    {
        private Conversation conversation;
        private readonly Entity reactingEntity;

        public SetReactiveConversationAction(Conversation conversation)
        {
            this.conversation = conversation;
        }

        public SetReactiveConversationAction(Conversation conversation, Entity reactingEntity)
        {
            this.conversation = conversation;
            this.reactingEntity = reactingEntity;
            conversation.OnDialoguOutcome += HandleDialogueOutcome;
        }

        public override void OnStart(Entity entity)
        {
            entity.GetState<ConversationState>().Conversation = conversation;
            ActionStatus = ActionStatus.Succeeded;
        }

        private void HandleDialogueOutcome(DialogueOutcome outcome)
        {
            switch (outcome)
            {
                case DialogueOutcome.Nice:
                    reactingEntity.GetState<RelationshipState>().PlayerOpinion++;
                    ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(reactingEntity, new UpdateMoodAction(Mood.Happy));
                    break;
                case DialogueOutcome.Mean:
                    reactingEntity.GetState<RelationshipState>().PlayerOpinion--;
                    ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(reactingEntity, new UpdateMoodAction(Mood.Angry));
                    break;
                default:
                    break;
            }
            reactingEntity.GetState<ConversationState>().Conversation = null;
        }

        public override void OnFrame(Entity entity)
        {
            //Do Nothing
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
