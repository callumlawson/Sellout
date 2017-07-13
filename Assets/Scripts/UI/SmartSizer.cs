using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.UI
{
    class SmartSizer : MonoBehaviour
    {
        [UsedImplicitly] public bool maxHeight;
        [UsedImplicitly] public bool maxWidth;

        [UsedImplicitly] [Range(0, 1)] public float maxScreenHeightPercentage;
        [UsedImplicitly] public float maxScreenHeightPixels;

        [UsedImplicitly] [Range(0, 1)] public float maxScreennWidthPercentage;
        [UsedImplicitly] public float maxScreenWidthPixels;

        [UsedImplicitly]
        void Start()
        {
            Resize();
            ResizeEventBehaviour.ScreenSizeUpdated += Resize;
        }

        //Only while working with UI.
//        void OnValidate()
//        {
//            Resize();
//        }

        private void Resize()
        {
            var panel = GetComponent<RectTransform>();

            var newSize = panel.sizeDelta;
            if (maxHeight)
            {
                var height = Mathf.Min(maxScreenHeightPixels, maxScreenHeightPercentage * Screen.height);
                newSize.y = height;
            }
            if (maxWidth)
            {
                var width = Mathf.Min(maxScreenWidthPixels, maxScreennWidthPercentage * Screen.width);
                newSize.x = width;
            }

            panel.sizeDelta = newSize;
        }
    }
}
