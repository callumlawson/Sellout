using System;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Visualizers
{
    public class BlackFader : MonoBehaviour {

        [UsedImplicitly] public Image Background;
        [UsedImplicitly] public Text Text;

        public void FadeToBlack(float timeInSeconds, string text = "", Action midFade = null, bool fadeIn = true)
        {
            if (fadeIn)
            {
                Background.DOFade(1.0f, timeInSeconds/4);
                Background.raycastTarget = true;
                DOTween.Sequence()
                    .SetDelay(timeInSeconds - timeInSeconds/4)
                    .OnComplete(() => Background.raycastTarget = false);
                Background.DOFade(0.0f, timeInSeconds/4).SetDelay(timeInSeconds - timeInSeconds/4);
            }
            else
            {
                Background.DOFade(1.0f, 0.0f);
                Background.raycastTarget = true;
                DOTween.Sequence()
                    .SetDelay(timeInSeconds - timeInSeconds / 2)
                    .OnComplete(() => Background.raycastTarget = false);
                Background.DOFade(0.0f, timeInSeconds/2).SetDelay(timeInSeconds/2);
            }

            if (Text != null && text != "")
            {
                if (fadeIn)
                {
                    Text.text = text;
                    Text.DOFade(1.0f, timeInSeconds/4).SetDelay(timeInSeconds/8);
                    Text.DOFade(0.0f, timeInSeconds/4).SetDelay(timeInSeconds - timeInSeconds/4 - timeInSeconds/8);
                }
                else
                {
                    Text.text = text;
                    Text.DOFade(1.0f, 0.0f);
                    Text.DOFade(0.0f, timeInSeconds/4).SetDelay(timeInSeconds/2);
                }
            }

            DOTween.Sequence().SetDelay(timeInSeconds/4).OnComplete(() =>
            {
                if (midFade != null)
                {
                    midFade.Invoke();
                }
            });
        }
    }
}
