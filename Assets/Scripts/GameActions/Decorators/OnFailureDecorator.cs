using System;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;

namespace Assets.Scripts.GameActions.Decorators
{
    public class OnFailureDecorator : GameAction
    {
        private readonly GameAction wrappedGameAction;
        private readonly Action onFailureAction;

        public OnFailureDecorator(GameAction wrappedGameAction, Action onFailureAction)
        {
            this.wrappedGameAction = wrappedGameAction;
            this.onFailureAction = onFailureAction;
        }

        public override void OnStart(Entity entity)
        {
            wrappedGameAction.OnStart(entity);
        }

        public override void OnFrame(Entity entity)
        {
            wrappedGameAction.OnFrame(entity);
            if (wrappedGameAction.IsComplete())
            {
                if (wrappedGameAction.ActionStatus == ActionStatus.Failed)
                {
                    onFailureAction.Invoke();
                }
                ActionStatus = wrappedGameAction.ActionStatus;
            }
        }

        public override void Pause()
        {
            wrappedGameAction.Pause();
        }

        public override void Unpause()
        {
            wrappedGameAction.Unpause();
        }

        public override string ToString()
        {
           return "[OnFailureDecorator] " + wrappedGameAction;
        }
    }
}
