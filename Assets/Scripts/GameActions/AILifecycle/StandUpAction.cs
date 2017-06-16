using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.States.AI;
using UnityEngine;

namespace Assets.Scripts.GameActions.AILifecycle
{
    class StandUpAction : GameAction, ICancellableAction
    {
        private float standUpTime = 1.0f;
        private float totalTime = 0.0f;

        public override void OnStart(Entity entity)
        {
            entity.GetState<PersonAnimationState>().TriggerAnimation(Util.AnimationEvent.SittingFinishTrigger);

            if (entity.HasState<LifecycleState>()) {
                entity.GetState<LifecycleState>().status = LifecycleState.LifecycleStatus.Standing;
            }
        }

        public override void OnFrame(Entity entity)
        {
            totalTime += Time.deltaTime;
            if (totalTime >= standUpTime)
            {
                ActionStatus = ActionStatus.Succeeded;
            }
        }

        public override void Pause()
        {
            //Do Nothing
        }

        public override void Unpause()
        {
            //Do Nothing;
        }

        public void Cancel()
        {
            //Do Nothing;
        }

        public bool IsCancellable()
        {
            return true;
        }
    }
}
