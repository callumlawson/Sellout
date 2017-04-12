using System;
using Assets.Framework.States;
using Assets.Scripts.Util;

namespace Assets.Scripts.States
{
    [Serializable]
    class PersonAnimationState : IState
    {
        public Action<AnimationEvent> TriggerAnimation;

        private AnimationEvent lastAnimationEvent;

        public PersonAnimationState()
        {
            TriggerAnimation += OnAnimationTriggered;
        }

        private void OnAnimationTriggered(AnimationEvent animationEvent)
        {
            lastAnimationEvent = animationEvent;
        }

        public override string ToString()
        {
            return string.Format("Last animation triggered: {0}", lastAnimationEvent);
        }
    }
}
