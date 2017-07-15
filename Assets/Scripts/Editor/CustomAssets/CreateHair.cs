using System.IO;
using Assets.Scripts.Util.NPC;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor.CustomAssets
{
    class CreateHair
    {
        [MenuItem("Assets/Create/Hair")]
        public static void CreateNewHair()
        {
            Hair asset = ScriptableObject.CreateInstance<Hair>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            AssetDatabase.CreateAsset(asset, path + "/NewHair.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
}
