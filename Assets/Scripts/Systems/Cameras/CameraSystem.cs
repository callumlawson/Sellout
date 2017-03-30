using System;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using System.Collections.Generic;
using Assets.Scripts.States.Cameras;
using UnityEngine;
using Assets.Scripts.Util.Cameras;

namespace Assets.Scripts.Systems.Cameras
{
    class CameraSystem : IReactiveEntitySystem, IFrameSystem, IInitSystem
    {
        private static CameraSystem _cameraSystem;
        public static CameraSystem GetCameraSystem()
        {
            return _cameraSystem;
        }

        public enum CameraMode
        {
            Following,
            Bar
        }
        
        private CameraMode CurrentMode = CameraMode.Following;
        private CameraMode TargetMode = CameraMode.Following;

        private Entity Camera;
        private CameraBar CameraBar;
        private CameraFollow CameraFollow;

        public void OnInit()
        {
            if (_cameraSystem != null)
            {
                Debug.LogError("Two camera systems are trying to initialize!");
                return;
            }

            _cameraSystem = this;
        }

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(CameraFollowState) };
        }

        public void OnEntityAdded(Entity entity)
        {
            if (Camera != null)
            {
                Debug.LogError("Tried to add two cameras!!");
                return;
            }

            Camera = entity;
            CameraFollow = entity.GameObject.GetComponent<CameraFollow>();
            CameraBar = entity.GameObject.GetComponent<CameraBar>();

            var cameraFollowTarget = entity.GetState<CameraFollowState>().GetFollowTarget();
            CameraFollow.SetTarget(cameraFollowTarget.transform);

            CurrentMode = CameraMode.Following;
            TargetMode = CameraMode.Following;
            CameraFollow.StartCameraBehaviour();
        }

        public void OnEntityRemoved(Entity entity)
        {
            //Do nothing.
        }

        public void SetCameraMode(CameraMode newMode)
        {
            if (newMode == CurrentMode)
            {
                return;
            }

            if (newMode == CameraMode.Following)
            {
                CameraBar.StopCameraBehaviour();
                TargetMode = CameraMode.Following;
            }
            else if (newMode == CameraMode.Bar)
            {
                CameraFollow.StopCameraBehaviour();
                TargetMode = CameraMode.Bar;
            }
        }

        public void OnFrame()
        {
            if (TargetMode == CameraMode.Following && CurrentMode == CameraMode.Bar)
            {
                if (CameraBar.IsFinished())
                {
                    CurrentMode = CameraMode.Following;
                    CameraFollow.StartCameraBehaviour();
                }
            }
            else if (TargetMode == CameraMode.Bar && CurrentMode == CameraMode.Following)
            {
                if (CameraFollow.IsFinished())
                {
                    CurrentMode = CameraMode.Bar;
                    CameraBar.StartCameraBehaviour();
                }
            }
        }
    }
}
