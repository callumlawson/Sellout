using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visualizers.NPCs
{
    class FaceVisualizer : MonoBehaviour
    {
        [SerializeField] [UsedImplicitly] private MeshFilter faceMesh;

        public void SetFace(Material face)
        {
            faceMesh.GetComponent<MeshRenderer>().sharedMaterial = face;
        }
    }
}
