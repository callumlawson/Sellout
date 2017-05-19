using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class ResizeEventBehaviour : MonoBehaviour
    {
        public static Action ScreenSizeUpdated;

        private Vector2 lastScreenSize;
        
        [UsedImplicitly]
        void Start()
        {
            lastScreenSize = new Vector2(Screen.width, Screen.height);
        }
        
        [UsedImplicitly]
        void Update()
        {
            if (Math.Abs(Screen.width - lastScreenSize.x) > 0.01f || Math.Abs(Screen.height - lastScreenSize.y) > 0.01f)
            {
                lastScreenSize.x = Screen.width;
                lastScreenSize.y = Screen.height;
                if (ScreenSizeUpdated != null)
                {
                    ScreenSizeUpdated.Invoke();
                }
            }
        }
    }
}
