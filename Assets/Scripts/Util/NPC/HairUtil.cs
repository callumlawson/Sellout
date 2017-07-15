using Assets.Framework.Util;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Util.NPC
{
    public enum HairType
    {
        None,
        Hair_Bartender,
        Hair_Q,
        Hair_Tolstoy,
        Hair_Jannet,
        Hair_McGraw,
        Hair_Ellie
    }

    class HairUtil
    {
        private static Dictionary<SpeciesType, List<HairType>> speciesHair = new Dictionary<SpeciesType, List<HairType>>() {
            { SpeciesType.Human, new List<HairType>() { HairType.None,
                                                        HairType.Hair_Bartender,
                                                        HairType.Hair_Q,
                                                        HairType.Hair_Tolstoy,
                                                        HairType.Hair_Jannet,
                                                        HairType.Hair_McGraw,
                                                        HairType.Hair_Ellie } },
            { SpeciesType.BirdPerson, new List<HairType>() { HairType.None } },
            { SpeciesType.ShadowPerson, new List<HairType>() { HairType.None } },
        };

        public static Hair GetHairAsset(HairType type)
        {
            if (type == HairType.None)
            {
                return null;
            }

            return AssetLoader.LoadScriptableObjectAsset<Hair>(type.ToString());
        }

        public static HairType GetRandomHiar(SpeciesType species)
        {
            var choice = Random.Range(0, speciesHair[species].Count);
            return speciesHair[species][choice];
        }
    }
}
