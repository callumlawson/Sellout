using System;
using Assets.Framework.States;

namespace Assets.Scripts.States.AI
{
    [Serializable]
    public class LifecycleState : IState
    {
        public enum LifecycleStatus
        {
            Offscreen,
            GoingToBar,
            Leaving,
            Sitting,
            Standing
        }

        public LifecycleStatus status;

        public LifecycleState()
        {
            status = LifecycleStatus.Offscreen;
        }

        public override string ToString()
        {
            return "LifecycleStatus: " + status;
        }
    }
}
