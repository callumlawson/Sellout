using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;

namespace Assets.Scripts.Systems
{
    //Updates the position in state from the scene. Move entities by setting their position the normal unity way.
    class PositionSystem : IReactiveEntitySystem, IFrameEntitySystem
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

        public void OnFrame(List<Entity> matchingEntities)
        {
            foreach (var entity in matchingEntities)
            {
                entity.GetState<PositionState>().Position = entity.GameObject.transform.position;
            }
        }

        public void OnEntityRemoved(Entity entity)
        {
            //Do nothing.
        }
    }
}
