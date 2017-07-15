using Assets.Framework.Util;
using UnityEngine;

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
        Face_Ellie
    }

    class FaceUtil
    {
        public static Face GetFace(FaceType type)
        {
            if (type == FaceType.None)
            {
                return null;
            }

            return AssetLoader.LoadScriptableObjectAsset<Face>(type.ToString());
        }
    }
}
