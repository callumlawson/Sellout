using System;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using UnityEngine;

namespace Assets.Scripts.GameActions.Decorators
{
    public class OnActionStatusDecorator : GameAction
    {
        private readonly GameAction wrappedGameAction;
        private readonly Action onSuccessAction;
        private readonly Action onFailureAction;

        public OnActionStatusDecorator(GameAction wrappedGameAction, Action onSuccessAction, Action onFailureAction)
        {
            this.wrappedGameAction = wrappedGameAction;
            this.onSuccessAction = onSuccessAction;
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
                if (wrappedGameAction.ActionStatus == ActionStatus.Succeeded)
                {
                    onSuccessAction.Invoke();
                }
                else if (wrappedGameAction.ActionStatus == ActionStatus.Failed)
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
           return "[OnActionStatusDecorator] " + wrappedGameAction;
        }
    }
}
