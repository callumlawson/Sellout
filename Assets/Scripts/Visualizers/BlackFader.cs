using System;
using System.Linq;
using Assets.Framework.States;
using Assets.Framework.Util;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Visualizers
{
    public class BlackFader : MonoBehaviour {

        [UsedImplicitly] public Image Background;
        [UsedImplicitly] public Text Text;

        [UsedImplicitly] public GameObject UIRoot;

        private GameObject defaultDialogueUI;
        private GameObject defaultDialogueLinesParent;
        private GameObject dialogueLineUI;

        private const float LineOffsetInSeconds = 0.3f;

        [UsedImplicitly]
        public void Start()
        {
            defaultDialogueUI = Instantiate(AssetLoader.LoadAsset(Prefabs.ListPanelUI));
            defaultDialogueUI.transform.SetParent(UIRoot.transform);
            defaultDialogueLinesParent = defaultDialogueUI.GetComponentInChildren<VerticalLayoutGroup>().gameObject;
            defaultDialogueUI.SetActive(false);
            dialogueLineUI = AssetLoader.LoadAsset(Prefabs.EndOfDayLineUI);
        }

        public void FadeToBlack(float delayTimeInSeconds, string text = "", Action midFade = null, bool fadeIn = true, bool endOfDay = false)
        {
            if (endOfDay)
            {
                defaultDialogueUI.SetActive(true);
                CleanUpDialogueLines();
                ShowPayments(delayTimeInSeconds);
                ShowStoryOutcomes(delayTimeInSeconds);
            }

            if (fadeIn)
            {
                Background.DOFade(1.0f, delayTimeInSeconds/4);
                Background.raycastTarget = true;
                DOTween.Sequence()
                    .SetDelay(delayTimeInSeconds - delayTimeInSeconds/4)
                    .OnComplete(() =>
                    {
                        Background.raycastTarget = false;
                    });
                Background.DOFade(0.0f, delayTimeInSeconds/4).SetDelay(delayTimeInSeconds - delayTimeInSeconds/4);
            }
            else
            {
                var dayMaterialColor = Text.GetComponent<Text>().color;
                Text.GetComponent<Text>().color = new Color(dayMaterialColor.r, dayMaterialColor.b, dayMaterialColor.g, 1.0f);
                var backgroundMaterialColor = Background.GetComponent<Image>().color; Background.GetComponent<Image>().color = 
                    new Color(backgroundMaterialColor.r, backgroundMaterialColor.b, backgroundMaterialColor.g, 1.0f);
                Background.raycastTarget = true;
                DOTween.Sequence()
                    .SetDelay(delayTimeInSeconds - delayTimeInSeconds / 2)
                    .OnComplete(() =>
                    {
                        Background.raycastTarget = false;
                    });
                Background.DOFade(0.0f, delayTimeInSeconds/2).SetDelay(delayTimeInSeconds/2);
            }

            if (Text != null && text != "")
            {
                if (fadeIn)
                {
                    Text.text = text;
                    Text.DOFade(1.0f, delayTimeInSeconds/4).SetDelay(delayTimeInSeconds/8);
                    Text.DOFade(0.0f, delayTimeInSeconds/4).SetDelay(delayTimeInSeconds - delayTimeInSeconds/4 - delayTimeInSeconds/8);
                }
                else
                {
                    Text.text = text;
                    Text.DOFade(1.0f, 0.0f);
                    Text.DOFade(0.0f, delayTimeInSeconds/4).SetDelay(delayTimeInSeconds/2);
                }
            }

            DOTween.Sequence().SetDelay(delayTimeInSeconds/4).OnComplete(() =>
            {
                if (midFade != null)
                {
                    midFade.Invoke();
                }
            });
        }

        private void ShowPayments(float timeInSeconds)
        {
            var payments = StaticStates.Get<PaymentTrackerState>().TodaysPayments;

            if (!payments.Any()) return;

            CreateLine("Ballance Sheet", timeInSeconds, 0, true);
            CreateLine("Drink Sales: " + payments[PaymentType.DrinkSale], timeInSeconds, 1);
            CreateLine("Ingredient Costs: " + payments[PaymentType.DrinkIngredient], timeInSeconds, 2);
            if (payments[PaymentType.DrugMoney] != 0)
            {
                CreateLine("Drug Money: " + payments[PaymentType.DrugMoney], timeInSeconds, 3);
            }
            CreateLine("Total: " + StaticStates.Get<MoneyState>().CurrentMoney, timeInSeconds, 4, false, TextAnchor.MiddleRight);
            CreateLine("", timeInSeconds, 4);
        }

        private void ShowStoryOutcomes(float timeInSeconds)
        {
            var outcomes = StaticStates.Get<OutcomeTrackerState>().TodaysOutcomes;

            if (!outcomes.Any()) return;

            CreateLine("Happenings:", timeInSeconds, 5, true);
            for (var index = 0; index < outcomes.Count; index++)
            {
                var outcome = outcomes[index];
                CreateLine(outcome, timeInSeconds, index + 6);
            }
        }

        private void CreateLine(string line, float fadetime, int lineNumber, bool yellow = false, TextAnchor alignment =  TextAnchor.MiddleLeft)
        {
            var lineGameObject = Instantiate(dialogueLineUI);
            lineGameObject.transform.SetParent(defaultDialogueLinesParent.transform);
            var text = lineGameObject.GetComponentInChildren<Text>();
            text.text = line;
            text.alignment = alignment;
            var textMaterialColor = text.GetComponent<Text>().color;
            if (yellow)
            {
                text.GetComponent<Text>().color = new Color(1.0f, 1.0f,
                   0.0f, 0.0f);
            }
            else
            {
                text.GetComponent<Text>().color = new Color(textMaterialColor.r, textMaterialColor.b,
                   textMaterialColor.g, 0.0f);
            }
            text.DOFade(1.0f, fadetime / 4).SetDelay(fadetime / 8 + lineNumber * LineOffsetInSeconds);
            text.DOFade(0.0f, fadetime / 4).SetDelay(fadetime - fadetime / 4 - fadetime / 8);
        }

        private void CleanUpDialogueLines()
        {
            foreach (Transform child in defaultDialogueLinesParent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
