using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;

namespace Assets.Scripts.Systems
{
    public class HealthSystem : ITickEntitySystem, IInitSystem
    {
        public List<Type> RequiredStates()
        {
            return new List<Type> {typeof(HealthState)};
        }

        public void OnInit()
        {
            UnityEngine.Debug.Log("The Health System Is Running");
        }

        public void Tick(List<Entity> matchingEntities)
        {
            foreach (var entity in matchingEntities)
            {
                var healthState = entity.GetState<HealthState>();
                healthState.DoDamage(10);  
                UnityEngine.Debug.Log("Health is " + healthState.CurrentHealth + " Selected entity is " + StaticStates.Get<SelectedState>().SelectedGameObject);
            }
        }
    }
}
