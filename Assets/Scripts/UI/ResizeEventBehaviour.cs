using System;
using UnityEngine;

namespace Assets.Scripts.UI
{
    class ResizeEventBehaviour : MonoBehaviour
    {
        public static Action ScreenSizeUpdated;

        private Vector2 lastScreenSize;
        
        void Start()
        {
            lastScreenSize = new Vector2(Screen.width, Screen.height);
        }
        
        void Update()
        {
            if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
            {
                lastScreenSize.x = Screen.width;
                lastScreenSize.y = Screen.height;
                ScreenSizeUpdated.Invoke();
            }
        }
    }
}
