using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using JetBrains.Annotations;

namespace Assets.Scripts.UI
{
    [UsedImplicitly]
    public class ResponseMouseOver : MonoBehaviour
    {
        private Image backgroundImage;

        [UsedImplicitly]
        void Start()
        {
            backgroundImage = GetComponent<Image>();
        }

        [UsedImplicitly]
        public void OnMouseEnter()
        {
            backgroundImage.DOFade(0.25f, 0.1f);
        }

        [UsedImplicitly]
        public void OnMouseLeave()
        {
            backgroundImage.DOFade(0.0f, 0.1f);
        }
    }
}
