using Assets.Framework.Entities;
using UnityEngine;
using Assets.Scripts.States;
using Assets.Scripts.Util.Clothing;

namespace Assets.Scripts.Visualizers.Clothing
{
    class NPCInitializer : MonoBehaviour, IEntityVisualizer
    {
        public void OnStartRendering(Entity entity)
        {
            var clothingState = entity.GetState<ClothingState>();
            var top = ClothingType.GetClothingAsset(clothingState.top);
            var bottom = ClothingType.GetClothingAsset(clothingState.bottom);

            var wearsClothing = GetComponentInChildren<ClothingWearer>();
            wearsClothing.SetTop(top);
            wearsClothing.SetBottom(bottom);
        }

        public void OnFrame()
        {
           
        }

        public void OnStopRendering(Entity entity)
        {
            
        }
    }
}
