using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.Systems;
using Assets.Scripts.Util.Dialogue;

namespace Assets.Scripts.GameActions.Dialogue
{
    public class ConversationAction : GameAction
    {
        private readonly Conversation conversation;
        private bool startedConversation;

        public ConversationAction(Conversation conversation)
        {
            this.conversation = conversation;
        }

        public override void OnFrame(Entity entity)
        {
            if (!startedConversation && !DialogueSystem.Instance.ConverstationActive)
            {
                conversation.Start(ConversationEnded, entity);
                startedConversation = true;
            }
        }

        public override void OnStart(Entity entity)
        {
            //Do nothing.
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
