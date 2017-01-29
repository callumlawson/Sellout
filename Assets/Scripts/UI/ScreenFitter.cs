using UnityEngine;

namespace Assets.Scripts.UI
{
    class ScreenFitter : MonoBehaviour
    {
        private float margin = 50;

        protected void Start()
        {
            var panel = GetComponent<RectTransform>();
            var newWidth = Mathf.Min(Screen.width - margin, panel.sizeDelta.x);
            var newHeight = Mathf.Min(Screen.height - margin, panel.sizeDelta.y);

            panel.sizeDelta = new Vector2(newWidth, newHeight);
        }
    }
}
