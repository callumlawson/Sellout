using Assets.Framework.Util;
using Assets.Scripts.Util.NPCVisuals;
using UnityEngine;
using System.Collections.Generic;

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
        ShadowPersonTop,
    }

    public enum ClothingBottomType
    {
        UniformBottom,
        BartenderBottom,

        BirdPersonBottom,
        ShadowPersonBottom,
    }

    public class ClothingUtil
    {
        private static Dictionary<SpeciesType, List<ClothingTopType>> speciesClothingTops = new Dictionary<SpeciesType, List<ClothingTopType>>()
        {
            { SpeciesType.Human, new List<ClothingTopType>() { ClothingTopType.UniformTopRed } },
            { SpeciesType.BirdPerson, new List<ClothingTopType>() { ClothingTopType.BirdPersonTop } },
            { SpeciesType.ShadowPerson, new List<ClothingTopType>() { ClothingTopType.ShadowPersonTop } },
        };

        private static Dictionary<SpeciesType, List<ClothingBottomType>> speciesClothingBottoms = new Dictionary<SpeciesType, List<ClothingBottomType>>()
        {
            { SpeciesType.Human, new List<ClothingBottomType>() { ClothingBottomType.UniformBottom } },
            { SpeciesType.BirdPerson, new List<ClothingBottomType>() { ClothingBottomType.BirdPersonBottom } },
            { SpeciesType.ShadowPerson, new List<ClothingBottomType>() { ClothingBottomType.ShadowPersonBottom } },
        };

        public static ClothingTop GetClothingAsset(ClothingTopType top)
        {
            return AssetLoader.LoadScriptableObjectAsset<ClothingTop>(top.ToString());
        }

        public static ClothingBottom GetClothingAsset(ClothingBottomType bottom)
        {
            return AssetLoader.LoadScriptableObjectAsset<ClothingBottom>(bottom.ToString());
        }

        public static ClothingTopType GetRandomTop(SpeciesType species)
        {
            var choice = Random.Range(0, speciesClothingTops[species].Count);
            return speciesClothingTops[species][choice];
        }

        public static ClothingBottomType GetRandomBottom(SpeciesType species)
        {
            var choice = Random.Range(0, speciesClothingBottoms[species].Count);
            return speciesClothingBottoms[species][choice];
        }
    }
}
