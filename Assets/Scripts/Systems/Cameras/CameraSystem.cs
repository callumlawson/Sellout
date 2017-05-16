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
        public enum CameraMode
        {
            Following,
            Bar,
            TargetPosition
        }

        private static CameraSystem cameraSystem;
        public static CameraSystem GetCameraSystem()
        {
            return cameraSystem;
        }
        
        private CameraMode currentMode = CameraMode.Following;
        private CameraMode targetMode = CameraMode.Following;
        private Entity camera;
        private CameraBar cameraBar;
        private CameraFollow cameraFollow;

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(CameraFollowState) };
        }

        public void OnInit()
        {
            if (cameraSystem != null)
            {
                Debug.LogError("Two camera systems are trying to initialize!");
                return;
            }

            cameraSystem = this;
        }

        public void SetCameraMode(CameraMode newMode)
        {
            if (newMode == currentMode)
            {
                return;
            }

            switch (newMode)
            {
                case CameraMode.Following:
                    cameraBar.StopCameraBehaviour();
                    targetMode = CameraMode.Following;
                    break;
                case CameraMode.Bar:
                    cameraFollow.StopCameraBehaviour();
                    targetMode = CameraMode.Bar;
                    break;
            }
        }

        public void OnEntityAdded(Entity entity)
        {
            if (camera != null)
            {
                Debug.LogError("Tried to add two cameras!!");
                return;
            }

            camera = entity;
            cameraFollow = entity.GameObject.GetComponent<CameraFollow>();
            cameraBar = entity.GameObject.GetComponent<CameraBar>();
            var cameraFollowTarget = entity.GetState<CameraFollowState>().GetFollowTarget();
            cameraFollow.SetTarget(cameraFollowTarget.transform);
            currentMode = CameraMode.Following;
            targetMode = CameraMode.Following;
            cameraFollow.StartCameraBehaviour();
        }

        public void OnFrame()
        {
            if (targetMode == CameraMode.Following && currentMode == CameraMode.Bar)
            {
                if (cameraBar.IsFinished())
                {
                    currentMode = CameraMode.Following;
                    cameraFollow.StartCameraBehaviour();
                }
            }
            else if (targetMode == CameraMode.Bar && currentMode == CameraMode.Following)
            {
                if (cameraFollow.IsFinished())
                {
                    currentMode = CameraMode.Bar;
                    cameraBar.StartCameraBehaviour();
                }
            }
        }

        public void OnEntityRemoved(Entity entity)
        {
            //Do nothing.
        }

 
    }
}
