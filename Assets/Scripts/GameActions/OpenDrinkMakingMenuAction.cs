using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.Systems;
using Assets.Scripts.Util.Events;

namespace Assets.Scripts.GameActions
{
    class OpenDrinkMakingMenuAction : GameAction
    {
        public override void OnFrame(Entity entity)
        {

        }

        public override void OnStart(Entity entity)
        {
            EventSystem.BroadcastEvent(new OpenDrinkMakingMenuEvent());
            ActionStatus = ActionStatus.Succeeded;
        }
    }
}
