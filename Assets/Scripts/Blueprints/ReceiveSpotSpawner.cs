using System.Collections.Generic;
using Assets.Framework.States;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using UnityEngine;
using Assets.Scripts.States.Bar;

namespace Assets.Scripts.Blueprints
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
                new TooltipState("A spot for patrons to place items.")
            };
        }
    }
}
