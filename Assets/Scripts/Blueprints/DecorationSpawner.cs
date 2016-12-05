using System.Collections.Generic;
using Assets.Framework.States;
using Assets.Scripts.States;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Blueprints
{
    //TODO need nicer system for this other than magic string.
    class DecorationSpawner : MonoBehaviour, IEntityBlueprint
    { 
        //Should grab this from the game object name probably.
        [UsedImplicitly] public string Prefab;

        public List<IState> EntityToSpawn()
        {
            return new List<IState>
            {
                new RotationState(transform.rotation),
                new PositionState(transform.position),
                new PrefabState(Prefab)
            };
        }
    }
}
