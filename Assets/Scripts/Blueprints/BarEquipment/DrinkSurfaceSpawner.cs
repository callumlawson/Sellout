using System.Collections.Generic;
using Assets.Framework.States;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using UnityEngine;
using JetBrains.Annotations;

namespace Assets.Scripts.Blueprints.BarEquipment
{
    [UsedImplicitly]
    public class DrinkSurfaceSpawner : MonoBehaviour, IEntityBlueprint
    {
        public List<IState> EntityToSpawn()
        {
            return new List<IState>
            {
                new PositionState(transform.position),
                new RotationState(transform.rotation),
                new PrefabState(Prefabs.DrinkSurface),
                new TooltipState("Your working area.")
            };
        }
    }
}
