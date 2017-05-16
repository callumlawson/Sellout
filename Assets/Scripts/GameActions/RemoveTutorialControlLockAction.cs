using System;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;

namespace Assets.Scripts.GameActions
{
    class RemoveTutorialControlLockAction : GameAction
    {
        public override void OnStart(Entity entity)
        {
            var playerState = StaticStates.Get<PlayerState>();
            playerState.TutorialControlLock = false;
            ActionStatus = ActionStatus.Succeeded;
        }

        public override void OnFrame(Entity entity)
        {
            //Do nothing
        }

        public override void Pause()
        {
            //Do nothing
        }

        public override void Unpause()
        {
            //Do nothing
        }
    }
}
