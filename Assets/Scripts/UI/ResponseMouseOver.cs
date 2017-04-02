using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Assets.Scripts.UI
{
    class ResponseMouseOver : MonoBehaviour
    {
        private Image backgroundImage;

        void Start()
        {
            backgroundImage = GetComponent<Image>();
        }

        public void OnMouseEnter()
        {
            backgroundImage.DOFade(0.25f, 0.0f);
        }

        public void OnMouseLeave()
        {
            backgroundImage.DOFade(0.0f, 0.1f);
        }
    }
}
