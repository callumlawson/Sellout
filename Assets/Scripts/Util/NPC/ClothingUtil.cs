using Assets.Framework.Util;
using Assets.Scripts.Util.NPCVisuals;
using UnityEngine;

namespace Assets.Scripts.Util.NPC
{
    public enum ClothingTopType
    {
        UniformTopRed,
        UniformTopBlue,
        UniformTopGray,
        UniformTopGreen,
        UniformTopOrange,
        BartenderTop
    }

    public enum ClothingBottomType
    {
        UniformBottom,
        BartenderBottom
    }

    public class ClothingUtil
    {
        public static ClothingTop GetClothingAsset(ClothingTopType top)
        {
            return AssetLoader.LoadScriptableObjectAsset<ClothingTop>(top.ToString());
        }

        public static ClothingBottom GetClothingAsset(ClothingBottomType bottom)
        {
            return AssetLoader.LoadScriptableObjectAsset<ClothingBottom>(bottom.ToString());
        }        
    }
}
