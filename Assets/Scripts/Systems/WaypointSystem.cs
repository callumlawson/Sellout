using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Framework.Util;
using Assets.Scripts.Blueprints;
using Assets.Scripts.States;
using Assets.Scripts.Util;

namespace Assets.Scripts.Systems
{
    class WaypointSystem : IReactiveEntitySystem, IEntityManager, IInitSystem
    {
        public static WaypointSystem Instance;

        private EntityStateSystem entitySystem;

        public void OnInit()
        {
            Instance = this;
        }

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

        private IEnumerable<Entity> GetFreeWaypointsThatSatisfyGoal(Goal goal)
        {
            return entitySystem.GetEntitiesWithState<GoalSatisfierState>()
                .Where(satisfierEntity => satisfierEntity.GetState<GoalSatisfierState>().SatisfiedGoals.Contains(goal))
                .Where(entity => entity.GetState<UserState>().IsFree());
        }

        public Entity GetWaypointThatSatisfiesGoalWithOcupant(Goal goal, Entity occupant)
        {
            return entitySystem
               .GetEntitiesWithState<GoalSatisfierState>()
               .Where(satisfierEntity => satisfierEntity.GetState<GoalSatisfierState>().SatisfiedGoals.Contains(goal))
               .FirstOrDefault(entity => Equals(entity.GetState<UserState>().User, occupant));
        }

        public Entity GetFreeWaypointThatSatisfiesGoal(Goal goal)
        {
            return GetFreeWaypointsThatSatisfyGoal(goal).FirstOrDefault();
        }

        public Entity GetAndUseWaypointThatSatisfiedGoal(Goal goal, Entity entity)
        {
            var waypoint = GetFreeWaypointsThatSatisfyGoal(goal).FirstOrDefault();
            if (waypoint == null)
            {
                return null;
            }
            waypoint.GetState<UserState>().User = entity;
            return waypoint;
        }

        public static void ReleaseWaypoint(Entity waypoint)
        {
            if (waypoint != null)
            {
                waypoint.GetState<UserState>().User = null;
            }
        }
    }
}
