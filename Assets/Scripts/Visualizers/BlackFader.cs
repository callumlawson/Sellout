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
            var payments = StaticStates.Get<PaymentTrackerState>();

            CreateLine("Ballance Sheet", timeInSeconds, 0, true);
            CreateLine("Drink Sales", timeInSeconds, 1, false, payments.GetPayment(PaymentType.DrinkSale).ToString());
            CreateLine("Ingredient Costs", timeInSeconds, 2, false, payments.GetPayment(PaymentType.DrinkIngredient).ToString());
            if (payments.GetPayment(PaymentType.DrugMoney) != 0)
            {
                CreateLine("Drug Money", timeInSeconds, 3, false, payments.GetPayment(PaymentType.DrugMoney).ToString());
            }
            CreateLine("", timeInSeconds, 4, false, "Current Total: " + StaticStates.Get<MoneyState>().CurrentMoney);
            CreateLine("", timeInSeconds, 4);
        }

        private void ShowStoryOutcomes(float timeInSeconds)
        {
            var outcomes = StaticStates.Get<OutcomeTrackerState>().TodaysOutcomes;

            if (!outcomes.Any()) return;

            CreateLine("Events", timeInSeconds, 5, true);
            for (var index = 0; index < outcomes.Count; index++)
            {
                var outcome = outcomes[index];
                CreateLine(outcome, timeInSeconds, index + 6);
            }
        }

        private void CreateLine(string lineLeft, float fadetime, int lineNumber, bool yellow = false, string lineRight = "")
        {
            var lineGameObject = Instantiate(dialogueLineUI);
            lineGameObject.transform.SetParent(defaultDialogueLinesParent.transform);
            var texts = lineGameObject.GetComponentsInChildren<Text>();
            var leftText = texts[0];
            var rightText = texts[1];

            leftText.text = lineLeft;
            var textMaterialColor = leftText.GetComponent<Text>().color;
            if (yellow)
            {
                leftText.GetComponent<Text>().color = new Color(1.0f, 1.0f,
                   0.0f, 0.0f);
            }
            else
            {
                leftText.GetComponent<Text>().color = new Color(textMaterialColor.r, textMaterialColor.b,
                   textMaterialColor.g, 0.0f);
            }
            leftText.DOFade(1.0f, fadetime / 4).SetDelay(fadetime / 8 + lineNumber * LineOffsetInSeconds);
            leftText.DOFade(0.0f, fadetime / 4).SetDelay(fadetime - fadetime / 4 - fadetime / 8);

            rightText.text = lineRight;
            rightText.GetComponent<Text>().color = new Color(textMaterialColor.r, textMaterialColor.b, textMaterialColor.g, 0.0f);
            rightText.DOFade(1.0f, fadetime / 4).SetDelay(fadetime / 8 + lineNumber * LineOffsetInSeconds);
            rightText.DOFade(0.0f, fadetime / 4).SetDelay(fadetime - fadetime / 4 - fadetime / 8);
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
