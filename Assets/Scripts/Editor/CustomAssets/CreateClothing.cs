using Assets.Scripts.Util.NPCVisuals;
using System.IO;
using Assets.Scripts.Util.NPC;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor.CustomAssets
{
    class CreateClothing
    {
        [MenuItem("Assets/Create/Clothing/Top")]
        public static void CreateClothingTop()
        {
            ClothingTop asset = ScriptableObject.CreateInstance<ClothingTop>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            AssetDatabase.CreateAsset(asset, path + "/NewClothingTop.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/Clothing/Bottom")]
        public static void CreateClothingBottom()
        {
            ClothingBottom asset = ScriptableObject.CreateInstance<ClothingBottom>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            AssetDatabase.CreateAsset(asset, path + "/NewClothingBottom.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
}
