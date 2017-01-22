using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.Systems;

namespace Assets.Scripts.GameActions
{
    class MakeDrinkAction : GameAction
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

        public bool IsCancellable()
        {
            return true;
        }
    }
}
