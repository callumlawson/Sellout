using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States.AI;
using Assets.Scripts.Util;

namespace Assets.Scripts.GameActions.AILifecycle
{
    class LeaveBarAction : GameAction, ICancellableAction
    {
        private readonly GoToPositionAction goToPosition;

        public LeaveBarAction()
        {
            goToPosition = new GoToPositionAction(Locations.OutsideDoorLocation());
        }

        public override void OnStart(Entity entity)
        {
            goToPosition.OnStart(entity);

            if (entity.HasState<LifecycleState>())
            {
                entity.GetState<LifecycleState>().status = LifecycleState.LifecycleStatus.Offscreen;
            }
        }

        public override void OnFrame(Entity entity)
        {
            goToPosition.OnFrame(entity);

            if (goToPosition.ActionStatus == ActionStatus.Succeeded)
            {
                ActionStatus = ActionStatus.Succeeded;
            }
            else if (goToPosition.ActionStatus == ActionStatus.Failed)
            {
                ActionStatus = ActionStatus.Failed;
            }
        }

        public override void Pause()
        {
            goToPosition.Pause();
        }

        public override void Unpause()
        {
            goToPosition.Unpause();
        }

        public void Cancel()
        {
            goToPosition.Cancel();
        }

        public bool IsCancellable()
        {
            return goToPosition.IsCancellable();
        }
    }
}
