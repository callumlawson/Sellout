using System.Collections.Generic;
using Assets.Framework.States;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Blueprints.BarEquipment
{
    public class DispenserBottleSpawner : MonoBehaviour, IEntityBlueprint
    {
        [UsedImplicitly] public Ingredient Ingredient;
        [UsedImplicitly] public Color Color;

        public List<IState> EntityToSpawn()
        {
            return new List<IState>
            {
                new DrinkState(new Dictionary<Ingredient, int>{ { Ingredient, 1 }}),
                 new ColorState(Color),
                new PositionState(transform.position),
                new RotationState(transform.rotation),
                new PrefabState(Prefabs.DispensingBottle),
                new InteractiveState(),
                new TooltipState("Dispenses " + Ingredient + "."),
                new InventoryState()
            };
        }
    }
}
