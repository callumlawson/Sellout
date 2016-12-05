using UnityEngine;

namespace Assets.Scripts.Util
{
    public class Interface : MonoBehaviour
    {
        public static Interface Instance;

        public GameObject TooltipRoot;
        public GameObject TooltipWindow;

        void Awake()
        {
            Instance = this;
        }
    }
}
