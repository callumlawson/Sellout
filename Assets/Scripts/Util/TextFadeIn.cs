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

        private float timeElapsed;

        private Color transparent;
        private Color opaque;
        private Text text;

        //TODO: Struggling to get the fade in smooth for some reason :(
        [UsedImplicitly]
        public void Start()
        {
            text = GetComponent<Text>();
            transparent = new Color(text.color.r, text.color.g, text.color.b, 0.0f);
            opaque = new Color(text.color.r, text.color.g, text.color.b, 1.0f);

            if (DoFadeOut)
            {
                StartCoroutine(FadeOut());
            }
        }

        [UsedImplicitly]
        public void Update()
        {
            timeElapsed += Time.deltaTime;
            var time = timeElapsed/DurationSeconds;
            text.color = Color.Lerp(transparent, opaque, time * time);
        }

        private IEnumerator FadeOut()
        {
            yield return new WaitForSeconds(DurationSeconds + FadeOutWaitTime);
            text.DOFade(1.0f, 1.0f);
            text.DOFade(0.0f, DurationSeconds).SetEase(Ease.InCubic);
            yield return new WaitForSeconds(DurationSeconds);
            text.enabled = false;
        }
    }
}
