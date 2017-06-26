using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;

namespace Assets.Scripts.GameActions
{
    class BusyAction : GameAction, ICancellableAction
    {

        public override void OnStart(Entity entity)
        {
           //Do Nothing
        }

        public override void OnFrame(Entity entity)
        {
            // Do Nothing
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
