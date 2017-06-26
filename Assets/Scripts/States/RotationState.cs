using System;
using Assets.Framework.States;
using UnityEngine;

namespace Assets.Scripts.States
{
    [Serializable]
    class RotationState : IState
    {
        public Quaternion Rotation;
        public Action<Quaternion> Teleport;

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
