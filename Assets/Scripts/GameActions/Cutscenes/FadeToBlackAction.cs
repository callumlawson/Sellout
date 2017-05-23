using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.Util;

namespace Assets.Scripts.GameActions.Cutscenes
{
    public class FadeToBlackAction : GameAction
    {
        public override void OnStart(Entity entity)
        {
            Interface.Instance.BlackFader.FadeToBlack(5.0f);
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
