using System.Collections.Generic;
using Assets.Framework.States;
using UnityEngine;

namespace Assets.Scripts.Blueprints
{
    //TODO need nicer system for this other than magic string.
    class DecorationSpawner : MonoBehaviour, IEntityBlueprint
    {
        public string Prefab;

        public List<IState> EntityToSpawn()
        {
            return new List<IState>
            {
                new PositionState(transform.position),
                new PrefabState(Prefab)
            };
        }
    }
}
