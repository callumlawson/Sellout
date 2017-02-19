using Assets.Framework.Systems;
using Assets.Scripts.States;
using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.Util;
using Assets.Scripts.Visualizers;

namespace Assets.Scripts.Systems.Waypoints
{
    class WaypointParentSystem : IEntityManager, IInitSystem
    {
        private EntityStateSystem entitySystem;

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(ChildWaypointsState) };
        }

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }

        public void OnInit()
        {
            // Do Nothing
        }

        public void OnEntityAdded(Entity entity)
        {
            var goalSatisfierState = entity.GetState<GoalSatisfierState>();
            var childWaypointsState = entity.GetState<ChildWaypointsState>();

            var childWaypoints = entity.GameObject.GetComponentsInChildren<WaypointVisualizer>();            
            for (var i = 0; i < childWaypoints.Length; i++)
            {
                var waypointChildId = childWaypoints[i].GetComponent<EntityIdComponent>().EntityId;
                var waypointChild = entitySystem.GetEntity(waypointChildId);
                childWaypointsState.AddChild(waypointChild);

                var childSatisfierState = waypointChild.GetState<GoalSatisfierState>();
                var childGoals = childSatisfierState.SatisfiedGoals;
                goalSatisfierState.SatisfiedGoals.AddRange(childGoals);
            }
        }

        public void OnEntityRemoved(Entity entity)
        {
            // Do Nothing
        }
    }
}
