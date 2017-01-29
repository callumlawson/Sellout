using UnityEngine;

namespace Assets.Scripts.Util.NPCVisuals
{
    public enum HairType
    {
        None,
        Bartender,
        Q,
        Tolstoy,
        Jannet,
        McGraw,
        Ellie
    }

    class HairUtil
    {
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
    }
}
