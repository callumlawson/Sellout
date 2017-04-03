﻿using Assets.Framework.States;
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
        }

        private void VisualizeDayTransition(int nextDay, bool fadeIn)
        {
            Day.text = string.Format(DayText, nextDay);

            if (fadeIn)
            {
                Day.DOFade(1.0f, FadeDuration/4);
                Background.DOFade(1.0f, FadeDuration/3);
            }
            else
            {
                var dayMaterialColor = Day.GetComponent<Text>().color;
                Day.GetComponent<Text>().color = new Color(dayMaterialColor.r, dayMaterialColor.b, dayMaterialColor.g, 1.0f);

                var backgroundMaterialColor = Background.GetComponent<Image>().color;
                Background.GetComponent<Image>().color = new Color(backgroundMaterialColor.r, backgroundMaterialColor.b, backgroundMaterialColor.g, 1.0f);
            }

            Background.raycastTarget = true;
            DOTween.Sequence().SetDelay(FadeDuration - FadeDuration / 4).OnComplete(() => Background.raycastTarget = false);

            Day.DOFade(0.0f, FadeDuration/4).SetDelay(FadeDuration - FadeDuration/4);
            Background.DOFade(0.0f, FadeDuration/4).SetDelay(FadeDuration - FadeDuration/4);
        }
    }
}