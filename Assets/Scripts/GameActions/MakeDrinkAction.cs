using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.Systems;

namespace Assets.Scripts.GameActions
{
    class MakeDrinkAction : GameAction, ICancellableAction
    {
        public override void OnFrame(Entity entity)
        {
            //Do Nothing
        }

        public override void OnStart(Entity entity)
        {
            EventSystem.StartDrinkMakingEvent.Invoke();
            EventSystem.EndDrinkMakingEvent += () =>
            {
                ActionStatus = ActionStatus.Succeeded;
            };
        }

        public override void Pause()
        {
            //Do Nothing
        }

        public override void Unpause()
        {
            //Do Nothing
        }

        public void Cancel()
        {
            EventSystem.EndDrinkMakingEvent.Invoke();
            ActionStatus = ActionStatus.Failed;
        }

        public bool IsCancellable()
        {
            return true;
        }
    }
}
