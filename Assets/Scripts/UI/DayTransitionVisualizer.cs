using Assets.Framework.States;
using Assets.Scripts.States;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class DayTransitionVisualizer : MonoBehaviour
    {
        [UsedImplicitly] public float FadeDuration;
        [UsedImplicitly] public Text Day;
        [UsedImplicitly] public Image Background;

        private const string DayText = "Day {0}";

        [UsedImplicitly]
        public void OnEnable()
        {
            StaticStates.Get<TimeState>().TriggerDayTransition += VisualizeDayTransition;
            Debug.Log("Registered callback");
        }

        public void VisualizeDayTransition(int nextDay)
        {
            Day.text = string.Format(DayText, nextDay);
            Day.DOFade(1.0f, FadeDuration/4);
            Day.DOFade(0.0f, FadeDuration/4).SetDelay(FadeDuration - FadeDuration/4);
            Background.DOFade(1.0f, FadeDuration/4);
            Background.DOFade(0.0f, FadeDuration/4).SetDelay(FadeDuration - FadeDuration / 4);
        }
    }
}