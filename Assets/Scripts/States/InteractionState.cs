using Assets.Framework.States;
using Assets.Scripts.Util;
using System;

namespace Assets.Scripts.States
{
    [Serializable]
    class InteractionState : IState
    {
        public readonly Message message;

        public InteractionState(Message message)
        {
            this.message = message;
        }
    }
}
