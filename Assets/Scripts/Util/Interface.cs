using Assets.Scripts.Visualizers;
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
        [UsedImplicitly] public GameObject MixologyBookUI;
        [UsedImplicitly] public BlackFader BlackFader;

        [UsedImplicitly]
        void Awake()
        {
            Instance = this;
        }
    }
}
