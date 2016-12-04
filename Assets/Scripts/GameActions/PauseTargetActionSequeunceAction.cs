﻿using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;

namespace Assets.Scripts.GameActions
{
    class PauseTargetActionSequeunceAction : GameAction
    {
        private Entity target;

        public PauseTargetActionSequeunceAction(Entity target)
        {
            this.target = target;
        }

        public override void OnFrame(Entity entity)
        {
            // Do nothing
        }

        public override void OnStart(Entity entity)
        {
            target.GetState<ActionBlackboardState>().Paused = true;
            ActionStatus = ActionStatus.Succeeded;
        }

        public override void Pause()
        {
            // Do nothing
        }

        public override void Unpause()
        {
            // Do nothing
        }
    }
}
