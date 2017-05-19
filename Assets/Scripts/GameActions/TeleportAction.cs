using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.GameActions
{
    public class TeleportAction : GameAction
    {
        private SerializableVector3 position;
        private Quaternion? rotation;

        public TeleportAction(SerializableVector3 position)
        {
            this.position = position;
        }

        public TeleportAction(Transform transform)
        {
            position = transform.position;
            rotation = transform.rotation;
        }

        public override void OnStart(Entity entity)
        {
            var positionState = entity.GetState<PositionState>();
            positionState.Teleport(position);

            if (rotation.HasValue)
            {
                entity.GameObject.transform.rotation = rotation.Value;
            }

            if (entity.HasState<PathfindingState>())
            {
                entity.GetState<PathfindingState>().ClearTarget();
            }

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
