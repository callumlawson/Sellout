using UnityEngine;

namespace Assets.Scripts.Util.Clothing
{
    public class NPCSpecs : ScriptableObject
    {
#pragma warning disable 0649
        [SerializeField] private Material faceMaterial;

        [SerializeField] private Mesh hairMesh;        
        [SerializeField] private Material hairMaterial;

        [SerializeField] private ClothingTop clothingTop;
        [SerializeField] private ClothingBottom clothingBottom;
#pragma warning restore 0649

        public Material GetFaceMaterial()
        {
            return faceMaterial;
        }

        public Mesh GetHairMesh()
        {
            return hairMesh;
        }

        public Material GetHairMaterial()
        {
            return hairMaterial;
        }

        public ClothingTop GetClothingTop()
        {
            return clothingTop;
        }

        public ClothingBottom getClothingBottom()
        {
            return clothingBottom;
        }
    }
}
