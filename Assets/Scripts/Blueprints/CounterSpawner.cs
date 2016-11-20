using System.Collections.Generic;
using Assets.Framework.States;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Blueprints
{
    class CounterSpawner : MonoBehaviour, IEntityBlueprint
    {
        public List<IState> EntityToSpawn()
        {
            return new List<IState>
            {
                new PositionState(transform.position),
                new PrefabState(Prefabs.Counter)
            };
        }
    }
}
