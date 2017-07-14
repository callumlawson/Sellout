using Assets.Scripts.Util.NPCVisuals;
using System.Collections.Generic;
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
        BartenderTop,

        BirdPersonTop,
        ShadowPersonTop
    }

    public enum ClothingBottomType
    {
        UniformBottom,
        BartenderBottom,

        BirdPersonBottom,
        ShadowPersonBottom
    };

    public class ClothingUtil
    {
        private static List<ClothingTopType> HumanClothingTops = new List<ClothingTopType>()
        {
            ClothingTopType.UniformTopRed,
            ClothingTopType.UniformTopBlue,
            ClothingTopType.UniformTopGray,
            ClothingTopType.UniformTopGreen,
            ClothingTopType.UniformTopOrange,
        };

        private static List<ClothingBottomType> HumanClothingBottoms = new List<ClothingBottomType>()
        {
            ClothingBottomType.UniformBottom,
            ClothingBottomType.BartenderBottom
        };

        public static ClothingTop GetClothingAsset(ClothingTopType top)
        {
            return GetClothingAsset<ClothingTop>(top.ToString());
        }

        public static ClothingBottom GetClothingAsset(ClothingBottomType bottom)
        {
            return GetClothingAsset<ClothingBottom>(bottom.ToString());
        }

        private static T GetClothingAsset<T>(string type) where T : ScriptableObject
        {
            var path = "NPCs/Clothing/" + type;
            var resource = Resources.Load<T>(path);
            return Object.Instantiate(resource);
        }

        public static ClothingTop GetRandomHumanClothingTop()
        {
            var choice = Random.Range(0, HumanClothingTops.Count);
            var choiceType = HumanClothingTops[choice];
            return GetClothingAsset(choiceType);
        }

        public static ClothingBottom GetRandomHumanClothingBottom()
        {
            var choice = Random.Range(0, HumanClothingBottoms.Count);
            var choiceType = HumanClothingBottoms[choice];
            return GetClothingAsset(choiceType);
        }
    }
}
