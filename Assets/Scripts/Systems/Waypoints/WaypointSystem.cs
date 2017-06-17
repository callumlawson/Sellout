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
using UnityEngine;

namespace Assets.Scripts.Systems
{
    class WaypointSystem : IInitSystem, IEntityManager, IReactiveEntitySystem
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

        public void ClearAllWaypoints()
        {
            var waypoints = entitySystem.GetEntitiesWithState<WaypointState>();
            foreach (var waypoint in waypoints)
            {
                waypoint.GetState<UserState>().ClearReserver();
                waypoint.GetState<UserState>().ClearUser();
            }
        }

        private IEnumerable<Entity> GetFreeWaypointsThatSatisfyGoal(Goal goal)
        {
            return entitySystem.GetEntitiesWithState<GoalSatisfierState>()
                .Where(satisfierEntity => satisfierEntity.GetState<GoalSatisfierState>().SatisfiedGoals.Contains(goal))
                .Where(entity => entity.GetState<UserState>().IsFree());
        }

        public Entity GetFreeWaypointThatSatisfyGoal(Goal goal)
        {
            return GetFreeWaypointsThatSatisfyGoal(goal).First();
        }

        private IEnumerable<Entity> GetFreeWaypointGroupThatSatisfiesGoals(List<Goal> goals)
        {
            return entitySystem.GetEntitiesWithState<ChildWaypointsState>()
                .Where(satisfierEntity => satisfierEntity.GetState<GoalSatisfierState>().SatisfiedGoals.Intersect(goals).Count() == goals.Count)
                .Where(entity => entity.GetState<UserState>().IsFree());
        }

        public Entity GetWaypointThatSatisfiesGoal(Goal goal)
        {
            var satisfiers = entitySystem.GetEntitiesWithState<GoalSatisfierState>().Where(satisfierEntity => satisfierEntity.GetState<GoalSatisfierState>().SatisfiedGoals.Contains(goal));
            if (satisfiers.Count() == 0)
            {
                return null;
            }

            return satisfiers.First();
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

        public IEnumerable<Entity> GetClosestFreeWaypointGroupThatSatisfiesGoals(Entity searcher, List<Goal> goals)
        {
            Entity closestWaypoint = null;
            var closestDistance = 0.0f;
            var freeWaypoints = GetFreeWaypointGroupThatSatisfiesGoals(goals);
            foreach (var waypoint in freeWaypoints)
            {
                var distance = DistanceBetweenEntities(waypoint, searcher);
                if (distance < closestDistance || closestWaypoint == null)
                {
                    closestWaypoint = waypoint;
                    closestDistance = distance;
                }
            }
            
            if (closestWaypoint == null)
            {
                return null;
            }

            var childWaypointsState = closestWaypoint.GetState<ChildWaypointsState>();

            if (childWaypointsState == null)
            {
                return new List<Entity> { closestWaypoint };
            }

            var children = closestWaypoint.GetState<ChildWaypointsState>().Children;
            var chosenChildren = new List<Entity>();
            for (var i = 0; i < goals.Count; i++)
            {
                var goal = goals[i];
                var childrenThatSatisfyGoal = children.Where(
                    satisfierEntity =>
                        satisfierEntity.GetState<GoalSatisfierState>().SatisfiedGoals.Contains(goal))
                            .Where(entity => !chosenChildren.Contains(entity))
                            .Where(entity => entity.GetState<UserState>().IsFree());
                var child = childrenThatSatisfyGoal.FirstOrDefault();
                if (child == null)
                {
                    Debug.LogError("Waypoint parent advertised child waypoints that didn't satisfy the goals!");
                    return null;
                }
                chosenChildren.Add(child);
            }
            return chosenChildren;
        }

        public static void StartUsingWaypoint(Entity waypoint, Entity user)
        {
            if (waypoint != null)
            {
                waypoint.GetState<UserState>().Use(user, "Wayponit System");
            }
        }

        public static void ReleaseWaypoint(Entity waypoint, Entity entityToRemove)
        {
            if (waypoint != null && waypoint.HasState<UserState>())
            {
                var userState = waypoint.GetState<UserState>();
                if (Equals(userState.Reserver, entityToRemove))
                {
                    userState.ClearReserver();
                }
                if (Equals(userState.User, entityToRemove))
                {
                    userState.ClearUser();
                }
            }
        }

        private float DistanceBetweenEntities(Entity entity, Entity otherEntity)
        {
            return entity.GetState<PositionState>().DistanceFrom(otherEntity.GetState<PositionState>());
        }
    }
}
