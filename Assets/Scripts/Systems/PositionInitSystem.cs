using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;

namespace Assets.Scripts.Systems
{
    class PositionInitSystem : IReactiveEntitySystem
    {
        public List<Type> RequiredStates()
        {
            return new List<Type> {typeof(PositionState), typeof(PrefabState)};
        }

        public void OnEntityAdded(Entity entity)
        {
            var position = entity.GetState<PositionState>().Position;
            entity.GameObject.transform.position = position;
        }

        public void OnEntityRemoved(Entity entity)
        {
            //Do nothing.
        }
    }
}
