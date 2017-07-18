using System;
using Assets.Framework.States;
using AnimationEvent = Assets.Scripts.Util.AnimationEvent;

namespace Assets.Scripts.States
{
    [Serializable]
    class PersonAnimationState : IState
    {
        public Action<AnimationEvent> TriggerAnimation;
        public Action ResetAnimationState = null;
            
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
