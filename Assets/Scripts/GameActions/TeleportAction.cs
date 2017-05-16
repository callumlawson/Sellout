using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.Util;

namespace Assets.Scripts.GameActions
{
    public class TeleportAction : GameAction
    {
        private SerializableVector3 position;

        public TeleportAction(SerializableVector3 position)
        {
            this.position = position;
        }

        public override void OnStart(Entity entity)
        {
            var positionState = entity.GetState<PositionState>();
            positionState.Teleport(position);
            ActionStatus = ActionStatus.Succeeded;
        }

        public override void OnFrame(Entity entity)
        {
            //Do Nothing
        }

        public override void Pause()
        {
            //Do Nothing
        }

        public override void Unpause()
        {
            //Do Nothing
        }
    }
}
