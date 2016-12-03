using System;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.Util.Dialogue;

namespace Assets.Scripts.GameActions
{
    class ConversationAction : GameAction
    {
        Conversation conversation;

        public ConversationAction(Conversation conversation)
        {
            this.conversation = conversation;
        }

        public override void OnFrame(Entity entity)
        {
            
        }

        public override void OnStart(Entity entity)
        {
            conversation.Start(ConversationEnded);
        }

        private void ConversationEnded()
        {
            ActionStatus = ActionStatus.Succeeded;
        }
    }
}
