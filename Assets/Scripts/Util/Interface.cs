using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public class Interface : MonoBehaviour
    {
        public static Interface Instance;

        [UsedImplicitly] public GameObject TooltipRoot;
        [UsedImplicitly] public GameObject TooltipWindow;
        [UsedImplicitly] public GameObject DyanmicUIRoot;

        [UsedImplicitly]
        void Awake()
        {
            Instance = this;
        }
    }
}
