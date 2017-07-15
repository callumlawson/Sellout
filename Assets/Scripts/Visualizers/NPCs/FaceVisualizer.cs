using Assets.Scripts.Util.NPC;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visualizers.NPCs
{
    class FaceVisualizer : MonoBehaviour
    {
        [SerializeField] [UsedImplicitly] private MeshFilter faceMesh;

        public void SetFace(Face face)
        {
            faceMesh.GetComponent<MeshFilter>().sharedMesh = face.mesh;
            faceMesh.GetComponent<MeshRenderer>().sharedMaterial = face.material;
        }
    }
}
