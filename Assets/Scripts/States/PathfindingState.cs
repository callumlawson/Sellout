using System;
using Assets.Framework.States;
using UnityEngine;

namespace Assets.Scripts.States
{
    [Serializable]
    class PathfindingState : IState
    {
        public SerializableVector3? Goal;

        public PathfindingState(Vector3? goal)
        {
            Goal = goal;
        }

        public override string ToString()
        {
            return string.Format("Pathfinding Goal: {0}", Goal);
        }
    }
}
