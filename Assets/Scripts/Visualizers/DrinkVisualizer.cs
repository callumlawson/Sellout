﻿using Assets.Framework.Entities;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visualizers
{
    class DrinkVisualizer : MonoBehaviour, IEntityVisualizer
    {
        [UsedImplicitly] public GameObject DrinkContents;
        [UsedImplicitly] public GameObject DrinkBottom;

        private DrinkState drinkState;
        private float halfDrinkHeight;
        private float initalLocalHeight;

        public void OnStartRendering(Entity entity)
        {
            initalLocalHeight = DrinkContents.gameObject.transform.localPosition.y;
            halfDrinkHeight = initalLocalHeight - DrinkBottom.gameObject.transform.localPosition.y;
            drinkState = entity.GetState<DrinkState>();
            drinkState.DrinkAmountChanged += OnDrinkChanged;
            OnDrinkChanged();
        }

        public void OnFrame()
        {
            //Do nothing
        }

        public void OnStopRendering(Entity entity)
        {
            drinkState.DrinkAmountChanged -= OnDrinkChanged;
        }

        private void OnDrinkChanged()
        {
            DrinkContents.SetActive(drinkState.GetTotalDrinkSize() != 0);
            UpdateLiquidHeight(drinkState.GetTotalDrinkSize());
        }

        private void UpdateLiquidHeight(int numDrinkUnits)
        {
            var fractionFull = (float) numDrinkUnits/Constants.MaxUnitsInDrink;
            DrinkContents.transform.localPosition = new Vector3(DrinkContents.transform.localPosition.x, initalLocalHeight - halfDrinkHeight*(1 - fractionFull), DrinkContents.transform.localPosition.z);
            DrinkContents.transform.localScale = new Vector3(1, fractionFull, 1);
        }
    }
}