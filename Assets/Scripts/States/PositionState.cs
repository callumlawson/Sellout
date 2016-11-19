using System;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    [Serializable]
    class PositionState : IState
    {
        public SerializableVector3 Position;

        public PositionState(SerializableVector3 position)
        {
            Position = position;
        }

        public override string ToString()
        {
            return string.Format("Position: {0}", Position);
        }
    }
}
