using System;
using UnityEngine;
using Assets.Framework.States;
using Assets.Framework.Entities;

namespace Assets.Scripts.States.Cameras
{
    [Serializable]
    class CameraFollowState : IState
    {
        private Entity EntityToFollow;

        public CameraFollowState(Entity entity)
        {
            EntityToFollow = entity;
        }

        public GameObject GetFollowTarget()
        {
            return EntityToFollow.GameObject;
        }
    }
}
