using System;
using UnityEngine;

namespace Assets.Scripts.Util.NPCVisuals
{
    [Serializable]
    public class ClothingBottom : ScriptableObject
    {
        public Mesh lowerSpineMesh;
        public Mesh hipMesh;
        public Mesh kneeMesh;

        public Material lowerSpine;
        public Material legs;
    }
}
