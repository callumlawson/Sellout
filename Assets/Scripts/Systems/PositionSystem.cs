using System;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using UnityEngine.AI;

namespace Assets.Scripts.Systems
{
    //Updates the position in state from the scene. Move entities by setting their position the normal unity way.
    class PositionSystem : IReactiveEntitySystem, IFrameEntitySystem
    {
        public System.Collections.Generic.List<Type> RequiredStates()
        {
            return new System.Collections.Generic.List<Type> {typeof(PositionState), typeof(PrefabState)};
        }

        public void OnEntityAdded(Entity entity)
        {
            var positionState = entity.GetState<PositionState>();
            Teleport(entity, positionState.Position);
            positionState.Teleport += pos => Teleport(entity, pos);
        }

        public void OnFrame(System.Collections.Generic.List<Entity> matchingEntities)
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

        private static void Teleport(Entity entity, SerializableVector3 position)
        {
            entity.GameObject.transform.position = position;
            if (entity.HasState<PathfindingState>())
            {
                entity.GameObject.GetComponent<NavMeshAgent>().Warp(position);
            }
        }
    }
}
