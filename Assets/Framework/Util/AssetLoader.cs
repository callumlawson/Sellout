using System;
using UnityEngine;

namespace Assets.Framework.Util
{
    public static class AssetLoader
    {
        private static AssetIndexObject AssetIndexObject;
        private static ScriptableObjectsAssetIndexObject ScriptableAssetIndexObject;

        public static GameObject LoadAsset(string prefabName)
        {
            if (AssetIndexObject == null)
            {
                AssetIndexObject = Resources.Load("AssetInfo/Assets.Framework.Util.AssetIndexObject") as AssetIndexObject;
            }

            var resolvedPath = ResolvePrefabPathFromIndex(prefabName);
            var prefabToSpawn = Resources.Load(resolvedPath) as GameObject;
            if (prefabToSpawn == null)
            {
                throw new Exception("Tried to load prefab named '" + prefabName + "' from path " + resolvedPath + " but failed.");
            }
            return prefabToSpawn;
        }

        public static T LoadScriptableObjectAsset<T>(string assetName) where T : ScriptableObject
        {
            if(ScriptableAssetIndexObject == null)
            {
                ScriptableAssetIndexObject = Resources.Load("AssetInfo/Assets.Framework.Util.ScriptableObjectsAssetIndexObject") as ScriptableObjectsAssetIndexObject;
            }

            if (ScriptableAssetIndexObject.AssetNames.Contains(assetName))
            {
                var index = ScriptableAssetIndexObject.AssetNames.IndexOf(assetName);
                var path = ScriptableAssetIndexObject.AssetPaths[index];
                var resource = Resources.Load<T>(path);
                return UnityEngine.Object.Instantiate(resource);
            }
            throw new Exception("Tried to resolve scriptable object with name: " + assetName + " but it was not in the asset index. Try re-indexing? Tools -> Rebuild Asset Index");
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
