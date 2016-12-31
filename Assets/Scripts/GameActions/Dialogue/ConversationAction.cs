using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.Util.Dialogue;

namespace Assets.Scripts.GameActions.Dialogue
{
    class ConversationAction : GameAction
    {
        private Conversation conversation;

        public ConversationAction(Conversation conversation)
        {
            this.conversation = conversation;
        }

        public override void OnFrame(Entity entity)
        {
            
        }

        public override void OnStart(Entity entity)
        {
            conversation.Start(ConversationEnded, entity);
        }

        private void ConversationEnded()
        {
            ActionStatus = ActionStatus.Succeeded;
        }

        public override void Pause()
        {
            conversation.Pause();
        }

        public override void Unpause()
        {
            conversation.Unpause();
        }
    }
}
