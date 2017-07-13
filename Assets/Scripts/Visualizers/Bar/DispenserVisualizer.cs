using Assets.Framework.Entities;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Visualizers.Bar
{
    class DispenserVisualizer : MonoBehaviour, IEntityVisualizer
    {
        [UsedImplicitly] public Renderer Label;

        [UsedImplicitly] public Material ColaLabel;
        [UsedImplicitly] public Material OrangeadeLabel;

        public void OnStartRendering(Entity entity)
        {
            var ingredient = entity.GetState<DrinkState>().GetContents().Keys.ToArray()[0];
            switch (ingredient)
            {
                case Ingredient.Cola:
                    Label.sharedMaterial = ColaLabel;
                    break;
                case Ingredient.Orangeade:
                    Label.sharedMaterial = OrangeadeLabel;
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
