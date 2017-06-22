using System.Collections.Generic;
using Assets.Framework.States;
using Assets.Scripts.States;
using Assets.Scripts.States.Bar;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Blueprints.BarEquipment
{
    public class BeerStackSpawner : MonoBehaviour, IEntityBlueprint
    {
        public List<IState> EntityToSpawn()
        {
            return new List<IState>
            {
                new PositionState(transform.position),
                new RotationState(transform.rotation),
                new PrefabState(Prefabs.BeerStack),
                new ItemStackState(),
                new InventoryState(),
                new VisibleSlotState(),
                new TooltipState("Dispenses beers.")
            };
        }
    }
}
