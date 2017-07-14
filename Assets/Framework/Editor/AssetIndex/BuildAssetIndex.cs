using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Framework.Util;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Assets.Framework.Editor.AssetIndex
{
    //NOTE: Not optimised! Lots of ways to make this more incremental.
    [UsedImplicitly]
    public class AssetIndexBuilder : AssetPostprocessor {

        [UsedImplicitly]
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var allPaths = importedAssets.Concat(deletedAssets).Concat(movedAssets).Concat(movedFromAssetPaths);
            if (!allPaths.All(IsAssetInfoFolder))
            {
                RebuildAssetIndex();
            }
        }

        private static bool IsAssetInfoFolder(string path)
        {
            return path.Contains("AssetInfo");
        }

        [MenuItem("Tools/Rebuild Asset Index")]
        private static void RebuildAssetIndex()
        {
            Debug.Log("Rebuilt Asset Index");
            var indexEntrys = GetAllAssetPathsFromFolder("Assets/Resources");
            var assetIndex = ScriptableObjectUtilities.GetScriptableObject<AssetIndexObject>();
            assetIndex.AssetNames = indexEntrys.Keys.ToList();
            assetIndex.AssetPaths = indexEntrys.Values.ToList();
            ScriptableObjectUtilities.SaveScriptableObject(assetIndex);

            // Scriptable Objects
            var scriptableObjectsIndexEntrys = GetAllScriptableAssetsPathsFromFolder("Assets/Resources");
            var scriptableObjectsAssetIndex = ScriptableObjectUtilities.GetScriptableObject<ScriptableObjectsAssetIndexObject>();
            scriptableObjectsAssetIndex.AssetNames = scriptableObjectsIndexEntrys.Keys.ToList();
            scriptableObjectsAssetIndex.AssetPaths = scriptableObjectsIndexEntrys.Values.ToList();
            ScriptableObjectUtilities.SaveScriptableObject(scriptableObjectsAssetIndex);
        }

        private static Dictionary<string, string> GetAllScriptableAssetsPathsFromFolder(string path)
        {
            if (path != "")
            {
                if (path.EndsWith("/"))
                {
                    path = path.TrimEnd('/');
                }
            }

            var dirInfo = new DirectoryInfo(path);
            var fileInf = dirInfo.GetFiles("*.asset", SearchOption.AllDirectories);

            //loop through directory loading the game object and checking if it has the component you want
            var prefabNameAndPaths = new Dictionary<string, string>();
            foreach (FileInfo fileInfo in fileInf)
            {
                var fullPath = fileInfo.FullName.Replace(@"\", "/");
                var assetPath = "Assets" + fullPath.Replace(Application.dataPath, "");
                assetPath = assetPath.Replace(".asset", "");
                assetPath = assetPath.Replace("Assets/Resources/", "");
                var name = fileInfo.Name.Replace(".asset", "");

                if (prefabNameAndPaths.ContainsKey(name))
                {
                    throw new Exception("More than one ScriptableObject has the name: " + name + " please rename one or remove it fromt the resorces folder.");
                }

                prefabNameAndPaths.Add(name, assetPath);
            }
            return prefabNameAndPaths;
        }

        private static Dictionary<string, string> GetAllAssetPathsFromFolder(string path)
        {
            if (path != "")
            {
                if (path.EndsWith("/"))
                {
                    path = path.TrimEnd('/');
                }
            }

            var dirInfo = new DirectoryInfo(path);
            var fileInf = dirInfo.GetFiles("*.prefab", SearchOption.AllDirectories);

            //loop through directory loading the game object and checking if it has the component you want
            var prefabNameAndPaths = new Dictionary<string, string>();
            foreach (FileInfo fileInfo in fileInf)
            {
                var fullPath = fileInfo.FullName.Replace(@"\", "/");
                var assetPath = "Assets" + fullPath.Replace(Application.dataPath, "");
                assetPath = assetPath.Replace(".prefab", "");
                assetPath = assetPath.Replace("Assets/Resources/", "");
                var name = fileInfo.Name.Replace(".prefab", "");

                if (prefabNameAndPaths.ContainsKey(name))
                {
                    throw new Exception("More than one prefab has the name: " + name + " please rename one or remove it fromt the resorces folder.");
                }

                prefabNameAndPaths.Add(name, assetPath);
            }
            return prefabNameAndPaths;
        }
    }
}
