using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Systems
{
    class RandomWanderSystem : ITickEntitySystem
    {
        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(RandomWandererFlagState) };
        }

        public void Tick(List<Entity> matchingEntities)
        {
            foreach (var entity in matchingEntities)
            {
                if (Random.value > 0.8)
                {
                    var xyPos = Random.insideUnitCircle * 15;
                    entity.GetState<PathfindingState>().Goal = new Vector3(xyPos.x, 0.0f, xyPos.y);
                }
            }
        }
    }
}