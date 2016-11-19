using UnityEngine;

namespace Assets.Scripts.Util
{
    public class InterfaceComponents : MonoBehaviour
    {
        public static InterfaceComponents Instance;

        public GameObject TooltipRoot;
        public GameObject TooltipWindow;

        void Awake()
        {
            Instance = this;
        }
    }
}
