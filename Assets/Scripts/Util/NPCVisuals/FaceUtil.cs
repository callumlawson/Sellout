using UnityEngine;

namespace Assets.Scripts.Util.NPCVisuals
{
    public enum FaceType
    {
        None,
        Bartender,
        Q,
        Tolstoy,
        Jannet,
        McGraw,
        Ellie
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
