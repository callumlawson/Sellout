using System;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Framework.States
{
    [Serializable]
    public class PositionState : IState
    {
        public SerializableVector3 Position;

        public PositionState(SerializableVector3 position)
        {
            Position = position;
        }

        public float DistanceFrom(PositionState otherPositionState)
        {
            return Vector3.Distance(Position, otherPositionState.Position);
        }

        public override string ToString()
        {
            return string.Format("Position: {0}", Position);
        }
    }
}
