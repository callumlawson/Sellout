using System;
using Assets.Framework.Entities;
using Assets.Framework.States;
namespace Assets.Scripts.States
{
    [Serializable]
    class CameraState : IState
    {
        public readonly Entity Camera;

        public CameraState(Entity camera)
        {
            Camera = camera;
        }

        public override string ToString()
        {
            return "Camera: " + Camera.EntityId;
        }
    }
}
