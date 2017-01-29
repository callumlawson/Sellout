using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Util
{
    class TextFadeIn : MonoBehaviour
    {
        [UsedImplicitly] public float DurationSeconds = 8.0f;

        [UsedImplicitly] public bool DoFadeOut;
        [UsedImplicitly] public int FadeOutWaitTime;

        [UsedImplicitly]
        public void Start()
        {
            var text = GetComponent<Text>();
            text.DOFade(0.0f, 0.0f);
            text.enabled = true;
            text.DOFade(1.0f, DurationSeconds).SetEase(Ease.InCubic);

            if (DoFadeOut)
            {
                StartCoroutine(FadeOut());
            }
        }

        private IEnumerator FadeOut()
        {
            yield return new WaitForSeconds(DurationSeconds + FadeOutWaitTime);
            var text = GetComponent<Text>();
            text.DOFade(1.0f, 1.0f);
            text.DOFade(0.0f, DurationSeconds).SetEase(Ease.InCubic);
            yield return new WaitForSeconds(DurationSeconds);
            text.enabled = false;
        }
    }
}
