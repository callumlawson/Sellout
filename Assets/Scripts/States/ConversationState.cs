using System;
using Assets.Framework.States;
using Assets.Scripts.Util.Dialogue;

namespace Assets.Scripts.States
{
    [Serializable]
    public class ConversationState : IState
    {
        public Conversation Conversation;

        public ConversationState(Conversation conversation)
        {
            Conversation = conversation;
        }

        public override string ToString()
        {
            return string.Format("Conversation: {0}", Conversation!=null);
        }
    }
}
