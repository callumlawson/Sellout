using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;

namespace Assets.Scripts.GameActions
{
    //TODO: Realised when making this it would be better to trigger animation events and
    //have the action succeed when the animation completes.
    class SetAnimationStateAction : GameAction
    {
        private readonly AnimationStatus animationStatus;

        public SetAnimationStateAction(AnimationStatus animationStatus)
        {
            this.animationStatus = animationStatus;
        }

        public override void OnStart(Entity entity)
        {
            entity.GetState<PersonAnimationState>().CurrentStatus = animationStatus;
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
    }
}
