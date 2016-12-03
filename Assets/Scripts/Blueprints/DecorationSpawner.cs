using System.Collections.Generic;
using Assets.Framework.States;
using UnityEngine;

namespace Assets.Scripts.Blueprints
{
    //TODO need nicer system for this other than magic string.
    class DecorationSpawner : MonoBehaviour, IEntityBlueprint
    {
#pragma warning disable 649
        public string Prefab;
#pragma warning restore 649

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
