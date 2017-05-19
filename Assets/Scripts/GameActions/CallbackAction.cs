using System;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;

namespace Assets.Scripts.GameActions
{
    class CallbackAction : GameAction, ICancellableAction 
    {
        private Action action;

        public CallbackAction(Action action)
        {
            this.action = action;
        }

        public override void OnFrame(Entity entity)
        {
            //Do Nothing
        }

        public override void OnStart(Entity entity)
        {
            if (action != null)
            {
                action.Invoke();
            }
            ActionStatus = ActionStatus.Succeeded;
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
            //Do Nothing
        }

        public bool IsCancellable()
        {
            return true;
        }
    }
}
