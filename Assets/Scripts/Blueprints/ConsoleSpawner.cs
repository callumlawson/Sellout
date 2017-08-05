using System.Collections.Generic;
using Assets.Framework.States;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Blueprints
{
    [UsedImplicitly]
    class ConsoleSpawner : MonoBehaviour, IEntityBlueprint
    {
        public List<IState> EntityToSpawn()
        {
            return new List<IState>
            {
                new RotationState(transform.rotation),
                new PositionState(transform.position),
                new PrefabState(Prefabs.Console),
                new TooltipState("Open the bar in the Morning. Leave at night.")
            };
        }
    }
}
