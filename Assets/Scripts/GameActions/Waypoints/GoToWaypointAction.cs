using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.GameActions.Waypoints
{
    class GoToWaypointAction : GameAction, ICancellableAction
    {
        private const float PositionTolerance = 0.5f;
        private PathfindingState pathfindingState;
        private Vector3 targetPosition;
        private float targetRotation;

        private bool destinationReached;
        private bool rotationStarted;

        public override void OnStart(Entity entity)
        {
            pathfindingState = entity.GetState<PathfindingState>();
            var targetWaypoint = entity.GetState<ActionBlackboardState>().TargetEntity;
            if (targetWaypoint != null)
            {
                targetPosition = targetWaypoint.GetState<PositionState>().Position;
                targetRotation = targetWaypoint.GetState<RotationState>().Rotation.eulerAngles.y;
                pathfindingState.SetNewTarget(targetPosition, targetRotation);
                pathfindingState.SetStoppingDistance(0f);
            }
            else
            {
                Debug.LogError("GotToWaypointAction failed as there was no waypoint. This shouldn't happen!");
                ActionStatus = ActionStatus.Failed;
            }
        }

        public override void OnFrame(Entity entity)
        {
            if (!destinationReached && Vector3.Distance(entity.GetState<PositionState>().Position, pathfindingState.GetTargetPosition().GetValueOrDefault()) <= PositionTolerance)
            {
                pathfindingState = entity.GetState<PathfindingState>();                
                destinationReached = true;
            }

            if (destinationReached && !rotationStarted)
            {
                rotationStarted = true;
                if (!pathfindingState.GetTargetRotation().HasValue)
                {
                    RotationFinished();
                }
                else
                {
                    pathfindingState.ClearPosition();
                    var rotationTime = GetRotationTime(entity.GameObject.transform.rotation.eulerAngles.y, pathfindingState.GetTargetRotation().Value);
                    entity.GameObject.transform.DORotate(new Vector3(0, pathfindingState.GetTargetRotation().Value, 0), rotationTime).OnComplete(RotationFinished);
                }
            }
            //TODO: Add timeout => Failure.
        }

        private float GetRotationTime(float start, float end)
        {
            var maxTime = 1.5f;
            if (start < 0) start += 360.0f;
            if (end < 0) end += 360.0f;

            var difference = Mathf.Abs(start - end) % 360;
            if (difference > 180.0f)
            {
                difference = 360.0f - difference;
            }
            return maxTime * (difference / 360.0f);
        }
        
        private void RotationFinished()
        {
            pathfindingState.ClearTarget();
            ActionStatus = ActionStatus.Succeeded;
        } 

        public override void Pause()
        {
            pathfindingState.SetPaused(true);
        }

        public override void Unpause()
        {
            pathfindingState.SetPaused(false);
        }

        public void Cancel()
        {
            pathfindingState.ClearTarget();
        }

        public bool IsCancellable()
        {
            return true;
        }
    }
}
