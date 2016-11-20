using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Framework.Util;
using Assets.Scripts.Blueprints;
using Assets.Scripts.States;

namespace Assets.Scripts.Systems
{
    class WaypointInitSystem : IReactiveEntitySystem, IEntityManager
    {
        private EntityStateSystem entitySystem;

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }

        public List<Type> RequiredStates()
        {
            return new List<Type> {typeof(WaypointState), typeof(BlueprintGameObjectState)};
        }

        public void OnEntityAdded(Entity entity)
        {
            var gameObject = entity.GetState<BlueprintGameObjectState>().BlueprintGameObject;
            var waypointSpawner = gameObject.GetComponent<WaypointSpawner>();
            Entity nextWaypoint = null;
            if (waypointSpawner != null)
            {
                var nextWaypointObject = waypointSpawner.NextWaypoint;
                if (nextWaypointObject != null)
                {
                    var entityIdOfNextWaypoint = nextWaypointObject.GetComponent<EntityIdComponent>().EntityId;
                    nextWaypoint = entitySystem.GetEntity(entityIdOfNextWaypoint);
                }
            }
            entity.GetState<WaypointState>().NextWaypoint = nextWaypoint;
        }

        public void OnEntityRemoved(Entity entity)
        {
            //Do nothing.
        }
    }
}
