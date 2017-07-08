using System.Collections.Generic;
using Assets.Framework.States;
using Assets.Scripts.States;
using Assets.Scripts.States.Bar;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Blueprints.BarEquipment
{
    public class ReceiveSpotSpawner : MonoBehaviour, IEntityBlueprint
    {
        public List<IState> EntityToSpawn()
        {
            return new List<IState>
            {
                new PositionState(transform.position),
                new RotationState(transform.rotation),
                new PrefabState(Prefabs.ReceiveSpot),
                new InventoryState(),
                new VisibleSlotState(),
                new BarEntityState(),
                new InteractiveState(),
                new TooltipState("A spot for patrons to place items.")
            };
        }
    }
}
