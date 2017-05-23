using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class BlackFader : MonoBehaviour {

    [UsedImplicitly] public Image Background;

    public void FadeToBlack(float timeInSeconds)
    {
        Background.DOFade(1.0f, timeInSeconds / 4);
        Background.raycastTarget = true;
        DOTween.Sequence().SetDelay(timeInSeconds - timeInSeconds / 4).OnComplete(() => Background.raycastTarget = false);
        Background.DOFade(0.0f, timeInSeconds / 4).SetDelay(timeInSeconds - timeInSeconds / 4);
    }
}
