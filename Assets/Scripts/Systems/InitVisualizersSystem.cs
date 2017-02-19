using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.Visualizers;

namespace Assets.Scripts.Systems
{
    class InitVisualizersSystem : IReactiveEntitySystem, IFrameEntitySystem
    {
        public List<Type> RequiredStates()
        {
            return new List<Type> {typeof(PrefabState)};
        }

        public void OnEntityAdded(Entity entity)
        {
            var components = entity.GameObject.GetComponents<IEntityVisualizer>();
            foreach (var component in components)
            {
                component.OnStartRendering(entity);
            }
        }

        public void OnFrame(List<Entity> matchingEntities)
        {
            foreach (var entity in matchingEntities)
            {
                var components = entity.GameObject.GetComponents<IEntityVisualizer>();
                foreach (var component in components)
                {
                    component.OnFrame();
                }
            }
        }

        public void OnEntityRemoved(Entity entity)
        {
            var components = entity.GameObject.GetComponents<IEntityVisualizer>();
            foreach (var component in components)
            {
                component.OnStopRendering(entity);
            }
        }
    }
}
