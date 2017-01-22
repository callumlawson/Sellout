using Assets.Scripts.Util.Clothing;
using UnityEngine;

namespace Assets.Scripts.Visualizers.Clothing
{
    class ClothingWearer : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private MeshFilter upperChestMesh;
        [SerializeField] private MeshFilter lowerChestMesh;
        [SerializeField] private MeshFilter leftShoulderMesh;
        [SerializeField] private MeshFilter rightShoulderMesh;
        [SerializeField] private MeshFilter leftElbowMesh;
        [SerializeField] private MeshFilter rightElbowMesh;

        [SerializeField] private MeshFilter lowerSpineMesh;
        [SerializeField] private MeshFilter leftHipMesh;
        [SerializeField] private MeshFilter rightHipMesh;
        [SerializeField] private MeshFilter leftKneeMesh;
        [SerializeField] private MeshFilter rightKneeMesh;
#pragma warning restore 0649

        public void SetTop(ClothingTop top)
        {
            PutOnTop(top);
        }

        public void SetBottom(ClothingBottom bottom)
        {
            PutOnBottom(bottom);
        }

        private void PutOnTop(ClothingTop top)
        {
            upperChestMesh.sharedMesh = top.upperChestMesh;
            lowerChestMesh.sharedMesh = top.lowerChestMesh;
            leftShoulderMesh.sharedMesh = top.shoulderMesh;
            rightShoulderMesh.sharedMesh = top.shoulderMesh;
            leftElbowMesh.sharedMesh = top.elbowMesh;
            rightElbowMesh.sharedMesh = top.elbowMesh;

            upperChestMesh.GetComponent<MeshRenderer>().sharedMaterial = top.chestMaterial;
            lowerChestMesh.GetComponent<MeshRenderer>().sharedMaterial = top.chestMaterial;
            leftShoulderMesh.GetComponent<MeshRenderer>().sharedMaterial = top.armsMaterial;
            rightShoulderMesh.GetComponent<MeshRenderer>().sharedMaterial = top.armsMaterial;
            leftElbowMesh.GetComponent<MeshRenderer>().sharedMaterial = top.armsMaterial;
            rightElbowMesh.GetComponent<MeshRenderer>().sharedMaterial = top.armsMaterial;
        }

        private void PutOnBottom(ClothingBottom bottom)
        {
            lowerSpineMesh.sharedMesh = bottom.lowerSpineMesh;
            leftHipMesh.sharedMesh = bottom.hipMesh;
            rightHipMesh.sharedMesh = bottom.hipMesh;
            leftKneeMesh.sharedMesh = bottom.kneeMesh;
            rightKneeMesh.sharedMesh = bottom.kneeMesh;

            lowerSpineMesh.GetComponent<MeshRenderer>().sharedMaterial = bottom.lowerSpine;
            leftHipMesh.GetComponent<MeshRenderer>().sharedMaterial = bottom.legs;
            rightHipMesh.GetComponent<MeshRenderer>().sharedMaterial = bottom.legs;
            leftKneeMesh.GetComponent<MeshRenderer>().sharedMaterial = bottom.legs;
            rightKneeMesh.GetComponent<MeshRenderer>().sharedMaterial = bottom.legs;
        }
    }
}
