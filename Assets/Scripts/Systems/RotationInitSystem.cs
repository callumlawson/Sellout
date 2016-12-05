using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;

namespace Assets.Scripts.Systems
{
    class RotationInitSystem : IReactiveEntitySystem
    {
        public List<Type> RequiredStates()
        {
            return new List<Type> {typeof(RotationState), typeof(PrefabState)};
        }

        public void OnEntityAdded(Entity entity)
        {
            var rotation = entity.GetState<RotationState>().Rotation;
            entity.GameObject.transform.rotation = rotation;
        }

        public void OnEntityRemoved(Entity entity)
        {
            //Do nothing.
        }
    }
}
