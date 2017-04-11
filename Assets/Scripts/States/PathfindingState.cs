using System;
using Assets.Framework.States;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.States
{
    [Serializable]
    class PathfindingState : IState
    {
        private SerializableVector3? targetPosition;
        private float? targetRotation;
        private bool paused;
        private float stoppingDistance;

        public PathfindingState(Vector3? targetPosition, float? targetRotation)
        {
            this.targetPosition = targetPosition;
            this.targetRotation = targetRotation;
        }

        public void SetNewTarget(Vector3 targetPosition, float? targetRotation = null)
        {
            this.targetPosition = targetPosition;
            this.targetRotation = targetRotation;
        }

        public void ClearTarget()
        {
            targetPosition = null;
            targetRotation = null;
        }

        public void ClearPosition()
        {
            targetPosition = null;
        }

        public bool GetPaused()
        {
            return paused;
        }

        public void SetPaused(bool paused)
        {
            this.paused = paused;
        }

        public float GetStoppingDistance()
        {
            return stoppingDistance;
        }

        public void SetStoppingDistance(float stoppingDistance)
        {
            this.stoppingDistance = stoppingDistance;
        }

        public SerializableVector3? GetTargetPosition()
        {
            return targetPosition;
        }

        public float? GetTargetRotation()
        {
            return targetRotation;
        }

        public override string ToString()
        {
            return string.Format("Pathfinding target: {0}, Angle: {1}", targetPosition, targetRotation);
        }
    }
}
