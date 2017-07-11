using Assets.Framework.Entities;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Visualizers
{
    class DispensingBottleVisualizer : MonoBehaviour, IEntityVisualizer
    {
        [UsedImplicitly] public GameObject DrinkContents;
        [UsedImplicitly] public Renderer Label;

        [UsedImplicitly] public Material RumLabel;
        [UsedImplicitly] public Material SyntholLabel;
        [UsedImplicitly] public Material VodkaLabel;

        public void OnStartRendering(Entity entity)
        {
            var colorState = entity.GetState<ColorState>();
            DrinkContents.GetComponent<Renderer>().material.color = colorState.Color;

            var ingredient = entity.GetState<DrinkState>().GetContents().Keys.ToArray()[0];
            switch (ingredient)
            {
                case Ingredient.Rum:
                    Label.sharedMaterial = RumLabel;
                    break;
                case Ingredient.Vodka:
                    Label.sharedMaterial = VodkaLabel;
                    break;
                case Ingredient.Synthol:
                    Label.sharedMaterial = SyntholLabel;
                    break;
            }
        }

        public void OnFrame()
        {
            //Do nothing
        }

        public void OnStopRendering(Entity entity)
        {
            //Do nothing.
        }
    }
}
