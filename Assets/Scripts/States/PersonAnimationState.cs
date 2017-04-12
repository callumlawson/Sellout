using System;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    public enum AnimationStatus
    {
        Moving, 
        Sitting
    }

    [Serializable]
    class PersonAnimationState : IState
    {
        public AnimationStatus CurrentStatus;

        public PersonAnimationState(AnimationStatus initStatus)
        {
            CurrentStatus = initStatus;
        }

        public override string ToString()
        {
            return string.Format("Animation Status: {0}", CurrentStatus);
        }
    }
}
