using Assets.Framework.Entities;
using Assets.Scripts.States;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visualizers
{
    [UsedImplicitly]
    class ColorStateVisualizer : MonoBehaviour, IEntityVisualizer
    {
        public void OnStartRendering(Entity entity)
        {
            var colorState = entity.GetState<ColorState>();

            var childRenderers = entity.GameObject.GetComponentsInChildren<Renderer>();
            foreach (var childRenderer in childRenderers)
            {
                childRenderer.material.color = colorState.Color;
            }

            var childLights = entity.GameObject.GetComponentsInChildren<Light>();
            foreach (var childLight in childLights)
            {
                childLight.color = colorState.Color;
            }
        }

        public void OnFrame()
        {
            //Do nothing
        }

        public void OnStopRendering(Entity entity)
        {
            //Do nothing
        }
    }
}
