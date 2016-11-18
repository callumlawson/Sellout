﻿using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.States;

namespace Assets.Scripts.Systems
{
    //Example!
    public class HealthSystem : ITickEntitySystem
    {
        public List<Type> RequiredStates()
        {
            return new List<Type> {typeof(HealthState)};
        }

        public void Tick(List<Entity> matchingEntities)
        {
            foreach (var entity in matchingEntities)
            {
                var healthState = entity.GetState<HealthState>();
                healthState.DoDamage(10);  
            }
        }
    }
}
