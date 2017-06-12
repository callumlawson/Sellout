using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Framework.Systems;
using Assets.Framework.Util;
using Assets.Scripts.Util;
using Assets.Scripts.Visualizers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Systems
{
    class DialogueSystem : IInitSystem
    {
        private readonly Interface interfaceMonobehaviour = Interface.Instance;

        private enum LineType
        {
            Dialogue,
            Response
        };

        //TODO: (Callum) This stateless static thing is nassssty. Rewrite.
        //Much better to pass this a conversation data object to run.
        public static DialogueSystem Instance;

        private GameObject defaultDialogueUI;
        private GameObject defaultDialogueLinesParent;
        private Text defaultSpeakerNameText;

        private GameObject barDialogueUI;
        private GameObject barDialogueLinesParent;
        private Text barSpeakerNameText;

        private GameObject dialogueLineUI;
        private GameObject responseLineUI;

        private GameObject currentDialogueUI;
        private GameObject currentDialogueLinesParent;
        private Text currentSpeakerNameText;
        
        private readonly List<GameObject> currentChoices = new List<GameObject>();
        private Sequence conversationTimeout;

        public bool ConverstationActive;
        
        public void OnInit()
        {
            InitUI();
            
            Instance = this;

            EventSystem.StartDrinkMakingEvent += OnStartMakingDrink;
            EventSystem.EndDrinkMakingEvent += OnEndMakingDrink;
        }

        private void OnEndMakingDrink()
        {
            StopDialogue();
            currentDialogueUI = defaultDialogueUI;
            currentDialogueLinesParent = defaultDialogueLinesParent;
            currentSpeakerNameText = defaultSpeakerNameText;
        }

        private void OnStartMakingDrink()
        {
            currentDialogueUI = barDialogueUI;
            currentDialogueLinesParent = barDialogueLinesParent;
            currentSpeakerNameText = barSpeakerNameText;
        }

        private void InitUI()
        {
            defaultDialogueUI = Object.Instantiate(AssetLoader.LoadAsset(Prefabs.DiagloguePanelUI));
            defaultDialogueUI.transform.SetParent(interfaceMonobehaviour.DyanmicUIRoot.transform);
            defaultDialogueLinesParent = defaultDialogueUI.GetComponentInChildren<VerticalLayoutGroup>().gameObject;
            defaultDialogueUI.SetActive(false);
            defaultSpeakerNameText = defaultDialogueUI.transform.Find("DiagloguePanelOuter/NamePanel").GetComponentInChildren<Text>();

            barDialogueUI = Object.Instantiate(AssetLoader.LoadAsset(Prefabs.BarDiagloguePanelUI));
            barDialogueUI.transform.SetParent(interfaceMonobehaviour.DyanmicUIRoot.transform);
            barDialogueLinesParent = barDialogueUI.GetComponentInChildren<VerticalLayoutGroup>().gameObject;
            barDialogueUI.SetActive(false);
            barSpeakerNameText = barDialogueUI.transform.Find("DiagloguePanelOuter/NamePanel").GetComponentInChildren<Text>();

            dialogueLineUI = AssetLoader.LoadAsset(Prefabs.DialogueLineUI);
            responseLineUI = AssetLoader.LoadAsset(Prefabs.ResponseLineUI);

            currentDialogueUI = defaultDialogueUI;
            currentDialogueLinesParent = defaultDialogueLinesParent;
            currentSpeakerNameText = defaultSpeakerNameText;
        }

        public void StartDialogue(string nameOfSpeaker, float timeoutInSeconds = 0.0f, Action onEndMethod = null)
        {
            CleanUpDialogueLines();
            ConverstationActive = true;
            WriteSpeakerName(nameOfSpeaker);
            ShowDialogue(true);
            StandardSoundPlayer.Instance.PlayPop();

            if (timeoutInSeconds > 0.1f)
            {
                conversationTimeout = DOTween.Sequence().SetDelay(timeoutInSeconds).OnComplete(() =>
                {
                    if (onEndMethod != null)
                    {
                        onEndMethod();
                    }
                    StopDialogue();
                });
            }
        }

        public void StopDialogue()
        {
            if (conversationTimeout != null)
            {
                conversationTimeout.Kill();
                conversationTimeout = null;
            }

            ConverstationActive = false;
            ShowDialogue(false);
        }

        public void PauseDialogue()
        {
            ShowDialogue(false);
        }

        public void UnpauseDialogue()
        {
            ShowDialogue(true);
        }

        public void NextPanel()
        {
            CleanUpDialogueLines();
        }

        public void WritePlayerDialogueLine(string line)
        {
            var textGameObject = CreateDialogueLine(line, LineType.Dialogue);
            var textField = textGameObject.GetComponent<Text>();
            textField.text = "     " + textField.text;
        }

        public void WritePlayerChoiceLine(string line, Action onSelected)
        {
            var textGameObject = CreateDialogueLine(line, LineType.Response);
            currentChoices.Add(textGameObject);

            var textField = textGameObject.GetComponentInChildren<Text>();
            textField.text = textField.text;

            var clickTrigger = textGameObject.GetComponent<EventTrigger>();
            var entry = new EventTrigger.Entry {eventID = EventTriggerType.PointerClick};
            entry.callback.AddListener(data =>
            {
                currentChoices.Remove(textGameObject);
                clickTrigger.triggers.Clear();
                DisableChoices();
                onSelected();
                StandardSoundPlayer.Instance.PlayClick();
            });
            clickTrigger.triggers.Add(entry);
        }

        public void WriteNPCLine(string line)
        {
            CreateDialogueLine(line, LineType.Dialogue);
        }

        private void WriteSpeakerName(string line)
        {
            currentSpeakerNameText.text = line;
        }

        private GameObject CreateDialogueLine(string line, LineType lineType)
        {
            var lineTemplate = lineType == LineType.Dialogue ? dialogueLineUI : responseLineUI;
            var lineGameObject = Object.Instantiate(lineTemplate);
            lineGameObject.transform.SetParent(currentDialogueLinesParent.transform);
            var text = lineGameObject.GetComponentInChildren<Text>();
            text.DOFade(0.0f, 0.0f);
            text.text = line;
            text.DOFade(1.0f, 0.5f);
            return lineGameObject;
        }

        private void DisableChoices()
        {
            foreach (var choice in currentChoices)
            {
                var text = choice.GetComponent<Text>();
                text.DOColor(new Color(0.1f, 0.1f, 0.1f), 2.0f);
                var trigger = choice.GetComponent<EventTrigger>();
                trigger.triggers.Clear();
            }
            currentChoices.Clear();
        }

        private void ShowDialogue(bool show)
        {
            if (show)
            {
                currentDialogueUI.SetActive(true);
                currentDialogueLinesParent.GetComponent<RectTransform>().localScale = new Vector3(0f, 0f, 0f);
                currentDialogueLinesParent.GetComponent<RectTransform>().DOScale(new Vector3(1, 1, 1), 0.3f).SetEase(Ease.InOutCubic);
            }
            else
            {
                currentDialogueUI.SetActive(false);
                //Closing the new conversation after it has started :(
                //dialogueLinesParent.GetComponent<RectTransform>().DOScale(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.InOutCubic).OnComplete(() => dialoguePanelUI.SetActive(false));
            }
        }

        private void CleanUpDialogueLines()
        {
            currentChoices.Clear();
            foreach (Transform child in currentDialogueLinesParent.transform)
            {
                Object.Destroy(child.gameObject);
            }
        }
    }
}
