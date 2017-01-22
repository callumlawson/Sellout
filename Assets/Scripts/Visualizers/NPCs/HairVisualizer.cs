using Assets.Scripts.Util.NPCVisuals;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visualizers.NPCs
{
    class HairVisualizer : MonoBehaviour
    {
        [SerializeField] [UsedImplicitly] private MeshFilter hairMesh;

        public void SetHair(Hair hair)
        {
            hairMesh.sharedMesh = hair.mesh;
            hairMesh.GetComponent<MeshRenderer>().sharedMaterial = hair.material;
        }
    }
}
