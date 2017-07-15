using Assets.Framework.Util;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Util.NPC
{
    public enum FaceType
    {
        None,
        Face_Bartender,
        Face_Q,
        Face_Tolstoy,
        Face_Jannet,
        Face_McGraw,
        Face_Ellie,

        Face_BirdPerson,
        Face_ShadowPerson
    }

    class FaceUtil
    {
        private static Dictionary<SpeciesType, List<FaceType>> speciesFaces = new Dictionary<SpeciesType, List<FaceType>>()
        {
            { SpeciesType.Human, new List<FaceType>() { FaceType.None } },
            { SpeciesType.BirdPerson, new List<FaceType>() { FaceType.Face_BirdPerson} },
            { SpeciesType.ShadowPerson, new List<FaceType>() { FaceType.Face_ShadowPerson } },
        };

        public static Face GetFace(FaceType type)
        {
            if (type == FaceType.None)
            {
                return null;
            }

            return AssetLoader.LoadScriptableObjectAsset<Face>(type.ToString());
        }

        internal static FaceType GetRandomFace(SpeciesType species)
        {
            var choice = Random.Range(0, speciesFaces[species].Count);
            return speciesFaces[species][choice];
        }
    }
}
