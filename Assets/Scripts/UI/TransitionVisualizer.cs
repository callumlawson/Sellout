using Assets.Framework.States;
using Assets.Scripts.States;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class TransitionVisualizer : MonoBehaviour
    {
        [UsedImplicitly] public float FadeDuration;
        [UsedImplicitly] public Text Text;
        [UsedImplicitly] public Image Background;
        [UsedImplicitly] public bool LastDayVisualizer;

        private const string DayText = "Day {0}";

        //I hope you never see this horror. But it's just a demo - so we are going to delete all of this, right? 
        [UsedImplicitly]
        public void OnEnable()
        {
            if (LastDayVisualizer)
            {
                StaticStates.Get<TimeState>().TriggerEndOfGame += () => VisualizeTransition("Thanks For Playing!", true, false);
            }
            else
            {
                StaticStates.Get<TimeState>().TriggerDayTransition += VisualizeTransition;
            }
        }

        private void VisualizeTransition(string message, bool fadeIn = true, bool fadeout = true)
        {
            Text.text = message;

            if (fadeIn)
            {
                Text.DOFade(1.0f, FadeDuration/4);
                Background.DOFade(1.0f, FadeDuration/3);
            }
            else
            {
                var dayMaterialColor = Text.GetComponent<Text>().color;
                Text.GetComponent<Text>().color = new Color(dayMaterialColor.r, dayMaterialColor.b, dayMaterialColor.g, 1.0f);

                var backgroundMaterialColor = Background.GetComponent<Image>().color;
                Background.GetComponent<Image>().color = new Color(backgroundMaterialColor.r, backgroundMaterialColor.b, backgroundMaterialColor.g, 1.0f);
            }

            if (fadeout)
            {
                Background.raycastTarget = true;
                DOTween.Sequence().SetDelay(FadeDuration - FadeDuration / 4).OnComplete(() => Background.raycastTarget = false);

                Text.DOFade(0.0f, FadeDuration / 4).SetDelay(FadeDuration - FadeDuration / 4);
                Background.DOFade(0.0f, FadeDuration / 4).SetDelay(FadeDuration - FadeDuration / 4);
            }
        }
    }
}