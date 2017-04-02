using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.UI
{
    class SmartSizer : MonoBehaviour
    {
        [UsedImplicitly] public bool maxHeight;
        [UsedImplicitly] public bool maxWidth;

        [Range(0, 1)]
        public float maxScreenHeightPercentage;
        public float maxScreenHeightPixels;

        [Range(0, 1)]
        public float maxScreennWidthPercentage;
        public float maxScreenWidthPixels;

        void Start()
        {
            Resize();
            ResizeEventBehaviour.ScreenSizeUpdated += Resize;
        }

        void OnValidate()
        {
            Resize();
        }

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
