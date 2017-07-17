using System.Collections.Generic;
using Assets.Framework.States;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using UnityEngine;
using Assets.Scripts.States.Bar;
using JetBrains.Annotations;

namespace Assets.Scripts.Blueprints.BarEquipment
{
    [UsedImplicitly]
    public class CubbySpawner : MonoBehaviour, IEntityBlueprint
    {
        public List<IState> EntityToSpawn()
        {
            return new List<IState>
            {
                new PositionState(transform.position),
                new RotationState(transform.rotation),
                new PrefabState(Prefabs.Cubby),
                new InventoryState(),
                new InteractiveState(),
                new VisibleSlotState(),
                new BarEntityState(),
                new TooltipState("A spot for placing your items.")
            };
        }
    }
}
