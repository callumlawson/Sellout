using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Framework.Util
{
    [Serializable]
    public class ScriptableObjectsAssetIndexObject : ScriptableObject
    {
        public List<string> AssetNames;
        public List<string> AssetPaths;
    }
}
