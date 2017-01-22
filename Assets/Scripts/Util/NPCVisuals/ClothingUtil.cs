﻿using System.IO;
using UnityEngine;

namespace Assets.Scripts.Util.NPCVisuals
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
            return Object.Instantiate(resource) as T;
        }
    }
}