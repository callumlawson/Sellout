using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;

namespace Assets.Scripts.GameActions.Cutscenes
{
    public class FadeToBlackAction : GameAction
    {
        public override void OnStart(Entity entity)
        {
            UnityEngine.Debug.Log("[Placeholder] Fading to black...");
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
