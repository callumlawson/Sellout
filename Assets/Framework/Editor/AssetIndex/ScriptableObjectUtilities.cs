using UnityEditor;
using UnityEngine;

namespace Assets.Framework.Editor.AssetIndex
{
    //TODO - Generalize further!
    public static class ScriptableObjectUtilities
    {
        public static T GetScriptableObject<T>() where T : ScriptableObject
        {
            var assetPath = GetPath<T>();

            var possibleAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
            if (possibleAsset)
            {
                return possibleAsset as T;
            }
            return CreateScriptableObject(assetPath, ScriptableObject.CreateInstance<T>());
        }

        private static T CreateScriptableObject<T>(string assetPath, T assetSettings) where T : ScriptableObject
        {
            AssetDatabase.CreateAsset(assetSettings, assetPath);
            AssetDatabase.SaveAssets();
            return assetSettings;
        }

        public static void SaveScriptableObject<T>(T settings) where T : ScriptableObject
        {
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }

        private static string GetPath<T>() where T : ScriptableObject
        {
            return "Assets/Resources/AssetInfo/" + typeof(T) + ".asset";
        }
    }
}