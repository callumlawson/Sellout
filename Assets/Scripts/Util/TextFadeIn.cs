using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Util
{
    class TextFadeIn : MonoBehaviour
    {
        [UsedImplicitly]
        public float DurationSeconds = 8.0f;

        [UsedImplicitly]
        public void Start()
        {
            var text = GetComponent<Text>();
            text.DOFade(0.0f, 0.0f);
            text.enabled = true;
            text.DOFade(1.0f, DurationSeconds).SetEase(Ease.InCubic);
        }
    }
}
