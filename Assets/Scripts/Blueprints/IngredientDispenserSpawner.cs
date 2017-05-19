using System.Collections.Generic;
using Assets.Framework.States;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Blueprints
{
    [UsedImplicitly]
    class IngredientDispenserSpawner : MonoBehaviour, IEntityBlueprint
    {
        // ReSharper disable once MemberCanBePrivate.Global
        [UsedImplicitly] public Ingredient Ingredient;
        // ReSharper disable once MemberCanBePrivate.Global
        [UsedImplicitly] public Color Color;

        public List<IState> EntityToSpawn()
        {
            return new List<IState>
            {
                new DrinkState(new Dictionary<Ingredient, int>{ { Ingredient, 1 }}),
                new ColorState(Color),
                new RotationState(transform.rotation),
                new PositionState(transform.position),
                new PrefabState(Prefabs.IngredientDispenser),
                new NameState(Ingredient.ToString(), 0.5f),
                new TooltipState("Dispenses " + Ingredient + ".")
            };
        }
    }
}
