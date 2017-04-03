using System;
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

        public static void ApplyActionRecursively(Transform transform, Action<Transform> function)
        {
            function(transform);
            foreach (Transform child in transform)
            {
                ApplyActionRecursively(child, function);
            }
        }
    }
}
