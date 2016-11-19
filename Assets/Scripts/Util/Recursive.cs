using UnityEngine;

namespace Assets.Scripts.Util
{
    public static class Recursive
    {
        public static void SetLayerRecursively(Transform transform, int layer)
        {
            transform.gameObject.layer = layer;
            foreach (Transform child in transform)
            {
                SetLayerRecursively(child, layer);
            }
        }
    }
}
