using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Util;

namespace Assets.Scripts.Systems
{
    class EnvironmentQualitySystem : IInitSystem, IEntityManager, IReactiveEntitySystem, ITickSystem
    {
        private EntityStateSystem entitySystem;

        private List<Entity> glasses;

        private float secondsBetweenUpdates;
        private float ticksSinceLastUpdate;
        private HashSet<Entity> glassesOnTables;
        private int numberOfGlassesOnTables; 

        public void OnInit()
        {
        }

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(DrinkState) };
        }

        public void OnEntityAdded(Entity entity)
        {
            var inventoryState = entity.GetState<InventoryState>();
            inventoryState.HierarchyUpdated += inventory => CheckIfChildOfStorageWaypoint(entity, inventory);
            glasses.Add(entity);
        }

        //public void Tick()
        //{
        //    ticksSinceLastUpdate += 1;
        //   if(ticksSinceLastUpdate * Constants.TickPeriodInSeconds > secondsBetweenUpdates)
        //    {
        //        ticksSinceLastUpdate = 0;
        //        UpdateEnvironmentQuality();
        //    }
        //}

        public void OnEntityRemoved(Entity entity)
        {
            var inventoryState = entity.GetState<InventoryState>();
            inventoryState.HierarchyUpdated -= CheckIfChildOfStorageWaypoint;
            glasses.Remove(entity);
        }

        private void CheckIfChildOfStorageWaypoint(Entity entity, InventoryState inventory)
        {
            if(inventory.Parent != null && inventory.Parent.HasState<WaypointState>())
            {
                glassesOnTables.Add(entity);
            }
            else
            {
                glassesOnTables.Remove(entity);
            }
            UpdateEnvironmentQuality(numberOfGlassesOnTables);
        }

        private void UpdateEnvironmentQuality(int numberOfGlassesOnTables)
        {
            var storedGlassCount = 0;
        }
    }
}
