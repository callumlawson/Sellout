using System;
using Assets.Framework.States;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.States
{
    [Serializable]
    class PathfindingState : IState
    {
        public SerializableVector3? TargetPosition;
        public bool Paused;
        public float StoppingDistance;

        public PathfindingState(Vector3? targetPosition)
        {
            TargetPosition = targetPosition;
        }

        public override string ToString()
        {
            return string.Format("Pathfinding target: {0}", TargetPosition);
        }
    }
}
