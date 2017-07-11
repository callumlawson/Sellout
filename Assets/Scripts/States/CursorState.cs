using System;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.Util;

namespace Assets.Scripts.States
{
    [Serializable]
    public class CursorState : IState
    {
        public Entity SelectedEntity;
        public Entity DebugEntity;
        public SerializableVector3 MousedOverPosition;

        public CursorState(Entity selectedEntity, SerializableVector3 mousedOverPosition)
        {
            SelectedEntity = selectedEntity;
            MousedOverPosition = mousedOverPosition;
        }
    }
}
