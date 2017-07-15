using System.IO;
using Assets.Scripts.Util.NPC;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor.CustomAssets
{
    class CreateFace
    {
        [MenuItem("Assets/Create/Face")]
        public static void CreateNewFace()
        {
            Face asset = ScriptableObject.CreateInstance<Face>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            AssetDatabase.CreateAsset(asset, path + "/NewFace.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
}
