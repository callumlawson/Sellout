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
            entity.GameObject.GetComponentInChildren<Renderer>().material.color = colorState.Color;
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
