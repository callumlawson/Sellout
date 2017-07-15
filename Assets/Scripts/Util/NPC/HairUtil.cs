using Assets.Framework.Util;
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
        public static Hair GetHairAsset(HairType type)
        {
            if (type == HairType.None)
            {
                return null;
            }

            return AssetLoader.LoadScriptableObjectAsset<Hair>(type.ToString());
        }
    }
}
