using System;
using UnityEngine;

namespace Assets.Framework.Util
{
    public static class AssetLoader
    {
        private static readonly AssetIndexObject AssetIndexObject = Resources.Load("AssetInfo/Assets.Framework.Util.AssetIndexObject") as AssetIndexObject;

        public static GameObject LoadAsset(string prefabName)
        {
            var resolvedPath = ResolvePrefabPathFromIndex(prefabName);
            var prefabToSpawn = Resources.Load(resolvedPath) as GameObject;
            if (prefabToSpawn == null)
            {
                throw new Exception("Tried to load prefab named '" + prefabName + "' from path " + resolvedPath + " but failed.");
            }
            return prefabToSpawn;
        }

        private static string ResolvePrefabPathFromIndex(string prefabName)
        {
            if (AssetIndexObject.AssetNames.Contains(prefabName))
            {
                var index = AssetIndexObject.AssetNames.IndexOf(prefabName);
                return AssetIndexObject.AssetPaths[index];
            }
            throw new Exception("Tried to resolve prefab with name: " + prefabName + " but it was not in the asset index. Try re-indexing? Tools -> Rebuild Asset Index");
        }
    }
}
