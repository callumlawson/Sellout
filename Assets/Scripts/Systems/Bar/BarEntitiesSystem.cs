using System;
using Assets.Framework.Systems;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Scripts.States.Bar;
using Assets.Framework.States;

namespace Assets.Scripts.Systems.Bar
{
    class BarEntitiesSystem : IInitSystem, IReactiveEntitySystem
    {
        private EntityStateSystem entitySystem;

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }

        public void OnInit()
        {
            var barEntitiesState = new BarEntities();
            StaticStates.Add(barEntitiesState);
        }

        public void OnEntityAdded(Entity entity)
        {
            if (entity.GetState<PrefabState>().PrefabName == "ReceiveSpot")
            {
                StaticStates.Get<BarEntities>().ReceiveSpot = entity;
            }
        }

        public void OnEntityRemoved(Entity entity)
        {
            // Do nothing... for now
        }

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(BarEntityState) };
        }
    }
}
