using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Framework.Util
{
    [Serializable]
    public class AssetIndexObject : ScriptableObject
    {
        public List<string> AssetNames;
        public List<string> AssetPaths;
    }

    [Serializable]
    public class ScriptableObjectsAssetIndexObject : AssetIndexObject
    {

    }
}
