﻿using System;
using UnityEngine;

namespace Assets.Scripts.Util.Clothing
{
    [Serializable]
    public class ClothingTop : ScriptableObject
    {
        public Mesh upperChestMesh;
        public Mesh lowerChestMesh;
        public Mesh shoulderMesh;
        public Mesh elbowMesh;
        
        public Material chestMaterial;
        public Material armsMaterial;
    }
}
