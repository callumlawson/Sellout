using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using System.Collections.Generic;
using UnityEngine.AI;

namespace Assets.Scripts.GameActions.Navigation
{
    public class SetMovementSpeedAction : GameAction, ICancellableAction
    {
        public enum MovementType
        {
            Walk,
            Run,
            Slow
        }

        private readonly static Dictionary<MovementType, float> movementSpeeds = new Dictionary<MovementType, float>()
        {
            { MovementType.Slow, 1.0f },
            { MovementType.Walk, 3.5f },
            { MovementType.Run, 7.0f }
        };

        private MovementType movementType;

        public SetMovementSpeedAction(MovementType movementType)
        {
            this.movementType = movementType;
        }

        public override void OnStart(Entity entity)
        {
            var newSpeed = movementSpeeds[movementType];
            entity.GameObject.GetComponent<NavMeshAgent>().speed = newSpeed;
            ActionStatus = ActionStatus.Succeeded;
        }

        public override void OnFrame(Entity entity)
        {
            // Do Nothing
        }

        public override void Pause()
        {
            // Do Nothing
        }

        public override void Unpause()
        {
            // Do Nothing;
        }

        public void Cancel()
        {
            // Do Nothing;
        }

        public bool IsCancellable()
        {
            return true;
        }
    }
}
