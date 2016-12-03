using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using UnityEngine;

namespace Assets.Scripts.GameActions
{
    class PauseAction : GameAction
    {
        private float pauseDuration;
        private float secondsPaused;

        public PauseAction(float pauseDuration)
        {
            this.pauseDuration = pauseDuration;
        }

        public override void OnStart(Entity entity)
        {
           //Do Nothing
        }

        public override void OnFrame(Entity entity)
        {
            secondsPaused += Time.deltaTime;
            if (secondsPaused >= pauseDuration)
            {
                ActionStatus = ActionStatus.Succeeded;
            }
        }
    }
}
