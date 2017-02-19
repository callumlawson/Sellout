using System.Collections.Generic;
using Assets.Framework.States;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Blueprints
{
    class BoothSpawner : MonoBehaviour, IEntityBlueprint
    {
        public List<IState> EntityToSpawn()
        {
            return new List<IState>
            {
                new PositionState(transform.position),
                new BlueprintGameObjectState(gameObject),
                new PrefabState(Prefabs.Booth),
                new GoalSatisfierState(new List<Goal> {Goal.Nothing}),
                new ChildWaypointsState()
            };
        }
    }
}
