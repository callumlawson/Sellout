using Assets.Framework.Entities;
using Assets.Scripts.States;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visualizers
{
    class DispensingBottleVisualizer : MonoBehaviour, IEntityVisualizer
    {
        [UsedImplicitly] public GameObject DrinkContents;

        public void OnStartRendering(Entity entity)
        {
            var colorState = entity.GetState<ColorState>();
            DrinkContents.GetComponent<Renderer>().material.color = colorState.Color;
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