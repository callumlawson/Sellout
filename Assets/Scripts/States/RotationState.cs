using Assets.Framework.States;
using UnityEngine;

namespace Assets.Scripts.States
{
    class RotationState : IState
    {
        public readonly Quaternion Rotation;

        public RotationState(Quaternion rotation)
        {
            Rotation = rotation;
        }

        public override string ToString()
        {
            return "Rotation: " + Rotation;
        }
    }
}
