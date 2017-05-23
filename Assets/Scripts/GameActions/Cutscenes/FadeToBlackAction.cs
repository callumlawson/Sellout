using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.Util;

namespace Assets.Scripts.GameActions.Cutscenes
{
    public class FadeToBlackAction : GameAction
    {
        private readonly float fadeTimeInSeconds;
        private readonly string message;

        public FadeToBlackAction(float fadeTimeInSeconds = 5.0f, string message = "")
        {
            this.fadeTimeInSeconds = fadeTimeInSeconds;
            this.message = message;
        }

        public override void OnStart(Entity entity)
        {
            Interface.Instance.BlackFader.FadeToBlack(fadeTimeInSeconds, message);
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
