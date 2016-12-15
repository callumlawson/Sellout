using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Framework.Entities;
using Assets.Framework.States;
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
            return new List<Type> {typeof(WaypointState), typeof(BlueprintGameObjectState), typeof(UserState)};
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
            //Do Nothing
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
               .FirstOrDefault(entity => Equals(entity.GetState<UserState>().Reserver, occupant));
        }

        public Entity GetFreeWaypointThatSatisfiesGoal(Goal goal)
        {
            return GetFreeWaypointsThatSatisfyGoal(goal).FirstOrDefault();
        }

        public Entity GetClosestFreeWaypointThatSatisfiesGoal(Entity searcher, Goal goal)
        {
            Entity closestWaypoint = null;
            var closestDistance = 0.0f;
            var freeWaypoints = GetFreeWaypointsThatSatisfyGoal(goal);
            foreach (var waypoint in freeWaypoints)
            {
                var distance = DistanceBetweenEntities(waypoint, searcher);
                if (distance < closestDistance || closestWaypoint == null)
                {
                    closestWaypoint = waypoint;
                    closestDistance = distance;
                }
            }
            return closestWaypoint;
        }

        public static void StartUsingWaypoint(Entity waypoint, Entity user)
        {
            if (waypoint != null)
            {
                waypoint.GetState<UserState>().User = user;
            }
        }

        public static void ReleaseWaypoint(Entity waypoint)
        {
            if (waypoint != null && waypoint.HasState<UserState>())
            {
                waypoint.GetState<UserState>().Reserver = null;
                waypoint.GetState<UserState>().User = null;
            }
        }

        private float DistanceBetweenEntities(Entity entity, Entity otherEntity)
        {
            return entity.GetState<PositionState>().DistanceFrom(otherEntity.GetState<PositionState>());
        }
    }
}
