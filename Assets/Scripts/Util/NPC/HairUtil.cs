using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Util.NPC
{
    public enum HairType
    {
        None,
        Bartender,
        Q,
        Tolstoy,
        Jannet,
        McGraw,
        Ellie,
        BirdPerson,
        ShadowPerson
    }

    class HairUtil
    {
        private static List<HairType> HumanHairTypes = new List<HairType>()
        {
            HairType.None
        };

        public static Hair GetHairAsset(HairType type)
        {
            if (type == HairType.None)
            {
                return null;
            }

            var path = "NPCs/Hair/Hair_" + type;
            var resource = Resources.Load<Hair>(path);
            return Object.Instantiate(resource);
        }

        public static HairType GetRandomHumanHair()
        {
            var choice = Random.Range(0, HumanHairTypes.Count);
            return HumanHairTypes[choice];
        }
    }
}
