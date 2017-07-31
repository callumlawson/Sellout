using System;
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

        [UsedImplicitly]
        public void Start()
        {
            defaultDialogueUI = Instantiate(AssetLoader.LoadAsset(Prefabs.ListPanelUI));
            defaultDialogueUI.transform.SetParent(UIRoot.transform);
            defaultDialogueLinesParent = defaultDialogueUI.GetComponentInChildren<VerticalLayoutGroup>().gameObject;
            defaultDialogueUI.SetActive(false);
            dialogueLineUI = AssetLoader.LoadAsset(Prefabs.DialogueLineUI);
        }

        public void FadeToBlack(float timeInSeconds, string text = "", Action midFade = null, bool fadeIn = true)
        {
            defaultDialogueUI.SetActive(true);
            if (fadeIn)
            {
                Background.DOFade(1.0f, timeInSeconds/4);
                Background.raycastTarget = true;
                DOTween.Sequence()
                    .SetDelay(timeInSeconds - timeInSeconds/4)
                    .OnComplete(() =>
                    {
                        Background.raycastTarget = false;
                        CleanUpDialogueLines();
                        foreach (var outcome in StaticStates.Get<OutcomeTrackerState>().TodaysOutcomes)
                        {
                            CreateLine(outcome);
                        }
                        defaultDialogueUI.SetActive(false);
                    });
                Background.DOFade(0.0f, timeInSeconds/4).SetDelay(timeInSeconds - timeInSeconds/4);
            }
            else
            {
                var dayMaterialColor = Text.GetComponent<Text>().color;
                Text.GetComponent<Text>().color = new Color(dayMaterialColor.r, dayMaterialColor.b, dayMaterialColor.g, 1.0f);
                var backgroundMaterialColor = Background.GetComponent<Image>().color; Background.GetComponent<Image>().color = 
                    new Color(backgroundMaterialColor.r, backgroundMaterialColor.b, backgroundMaterialColor.g, 1.0f);
                Background.raycastTarget = true;
                DOTween.Sequence()
                    .SetDelay(timeInSeconds - timeInSeconds / 2)
                    .OnComplete(() =>
                    {
                        defaultDialogueUI.SetActive(false);
                        Background.raycastTarget = false;
                    });
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

        private void CreateLine(string line)
        {
            var lineGameObject = Instantiate(dialogueLineUI);
            lineGameObject.transform.SetParent(defaultDialogueLinesParent.transform);
            var text = lineGameObject.GetComponentInChildren<Text>();
            text.DOFade(0.0f, 0.0f);
            text.text = line;
            text.DOFade(1.0f, 0.5f);
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
