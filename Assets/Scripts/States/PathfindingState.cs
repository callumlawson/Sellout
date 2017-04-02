using System;
using Assets.Framework.States;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.States
{
    [Serializable]
    class PathfindingState : IState
    {
        private SerializableVector3? TargetPosition;
        private float? TargetRotation;
        private bool Paused;
        private float StoppingDistance;

        public PathfindingState(Vector3? targetPosition, float? targetRotation)
        {
            TargetPosition = targetPosition;
            TargetRotation = targetRotation;
        }

        public void SetNewTarget(Vector3 targetPosition, float? targetRotation = null)
        {
            TargetPosition = targetPosition;
            TargetRotation = targetRotation;
        }

        public void ClearTarget()
        {
            TargetPosition = null;
            TargetRotation = null;
        }

        public bool GetPaused()
        {
            return Paused;
        }

        public void SetPaused(bool paused)
        {
            Paused = paused;
        }

        public float GetStoppingDistance()
        {
            return StoppingDistance;
        }

        public void SetStoppingDistance(float stoppingDistance)
        {
            StoppingDistance = stoppingDistance;
        }

        public SerializableVector3? GetTargetPosition()
        {
            return TargetPosition;
        }

        public float? GetTargetRotation()
        {
            return TargetRotation;
        }

        public override string ToString()
        {
            return string.Format("Pathfinding target: {0}", TargetPosition);
        }
    }
}
