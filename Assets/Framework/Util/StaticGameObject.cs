using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Framework.Util
{
    public class StaticGameObject : MonoBehaviour
    {
        public static StaticGameObject Instance;

        [UsedImplicitly]
        void Awake()
        {
            Instance = this;
        }
    }
}
