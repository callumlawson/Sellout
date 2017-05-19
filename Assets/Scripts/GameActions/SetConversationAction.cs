using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.Util.Dialogue;

namespace Assets.Scripts.GameActions
{
    class SetConversationAction : GameAction, ICancellableAction 
    {
        private readonly Conversation conversation;

        public SetConversationAction(Conversation conversation)
        {
            this.conversation = conversation;
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
