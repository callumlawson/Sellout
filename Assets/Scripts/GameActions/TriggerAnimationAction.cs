using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.GameActions
{
    //TODO: Realised when making this it would be better to trigger animation events and
    //have the action succeed when the animation completes.
    class TriggerAnimationAction : GameAction, ICancellableAction
    {
        private readonly Util.AnimationEvent animationEvent;

        public TriggerAnimationAction(Util.AnimationEvent animationEvent)
        {
            this.animationEvent = animationEvent;
        }

        public override void OnStart(Entity entity)
        {
            entity.GetState<PersonAnimationState>().TriggerAnimation(animationEvent);
            ActionStatus = ActionStatus.Succeeded;
        }

        public override void OnFrame(Entity entity)
        {
            //Do nothing.
        }

        public override void Pause()
        {
            //Do nothing.
        }

        public override void Unpause()
        {
            //Do nothing.
        }

        public void Cancel()
        {
            //Do nothing.
        }

        public bool IsCancellable()
        {
            return true;
        }
    }
}
