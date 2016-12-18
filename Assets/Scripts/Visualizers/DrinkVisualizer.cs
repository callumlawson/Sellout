using Assets.Framework.Entities;
using Assets.Scripts.States;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visualizers
{
    class DrinkVisualizer : MonoBehaviour, IEntityVisualizer
    {
        [UsedImplicitly] public GameObject DrinkContents;

        private DrinkState drinkState;

        public void OnStartRendering(Entity entity)
        {
            drinkState = entity.GetState<DrinkState>();
            drinkState.DrinkAmountChanged += OnDrinkChanged;
            OnDrinkChanged();
        }

        private void OnDrinkChanged()
        {
            DrinkContents.SetActive(drinkState.GetTotalDrinkSize() != 0);
        }

        public void OnFrame()
        {
            //Do nothing
        }

        public void OnStopRendering(Entity entity)
        {
            drinkState.DrinkAmountChanged -= OnDrinkChanged;
        }
    }
}