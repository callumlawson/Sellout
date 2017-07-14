using UnityEngine;

namespace Assets.Scripts.Util.NPC
{
    public enum FaceType
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

    class FaceUtil
    {
        public static Material GetFaceMaterial(FaceType type)
        {
            if (type == FaceType.None)
            {
                return null;
            }

            var path = "NPCs/Faces/Materials/Face_" + type;
            return Resources.Load<Material>(path);
        }
    }
}
