using Assets.Framework.Entities;
using UnityEngine;
using Assets.Scripts.States;
using Assets.Scripts.Util.NPC;
using Assets.Scripts.Util.NPCVisuals;

namespace Assets.Scripts.Visualizers.NPCs
{
    class CharacterVisualInitializer : MonoBehaviour, IEntityVisualizer
    {
        public void OnStartRendering(Entity entity)
        {
            var clothingState = entity.GetState<ClothingState>();
            var top = ClothingUtil.GetClothingAsset(clothingState.top);
            var bottom = ClothingUtil.GetClothingAsset(clothingState.bottom);

            var clothingVisualizer = GetComponentInChildren<ClothingVisualizer>();
            clothingVisualizer.SetTop(top);
            clothingVisualizer.SetBottom(bottom);

            var hairState = entity.GetState<HairState>();
            var hair = HairUtil.GetHairAsset(hairState.hair);
            if (hair != null)
            {
                var hairVisualizer = GetComponentInChildren<HairVisualizer>();
                hairVisualizer.SetHair(hair);
            }

            var faceState = entity.GetState<FaceState>();
            var face = FaceUtil.GetFace(faceState.face);
            if (face != null)
            {
                var faceVisualizer = GetComponentInChildren<FaceVisualizer>();
                faceVisualizer.SetFace(face);
            }
        }

        public void OnFrame()
        {
           
        }

        public void OnStopRendering(Entity entity)
        {
            
        }
    }
}
