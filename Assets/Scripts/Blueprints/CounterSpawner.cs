using System.Collections.Generic;
using Assets.Framework.States;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Blueprints
{
    [UsedImplicitly]
    class CounterSpawner : MonoBehaviour, IEntityBlueprint
    {
        public List<IState> EntityToSpawn()
        {
            return new List<IState>
            {
                new RotationState(transform.rotation),
                new PositionState(transform.position),
                new PrefabState(Prefabs.Counter),
                new CounterState(),
                new TooltipState("Make drinks for customers."),
                new InteractiveState()
            };
        }
    }
}
