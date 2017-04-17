using System;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    [Serializable]
    class InteractiveState : IState
    {
        public bool CurrentlyInteractive;

        public InteractiveState(bool interactive = true)
        {
            CurrentlyInteractive = interactive;
        }

        public override string ToString()
        {
            return string.Format("Interactive: {0}", CurrentlyInteractive);
        }
    }
}
