﻿using System;
using System.Collections.Generic;
using Assets.Framework.Systems;
using Assets.Framework.Util;
using Assets.Scripts.Util;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Systems
{
    class DialogueSystem : IInitSystem
    {
        //TODO: (Callum) This stateless static thing is nassssty. Rewrite.
        //Much better to pass this a conversation data object to run.
        public static DialogueSystem Instance;

        private GameObject dialoguePanelUI;
        private GameObject dialogueLinesParent;
        private GameObject dialogueLineUI;
        private readonly List<GameObject> currentChoices = new List<GameObject>();

        public bool ConverstationActive;

        public void OnInit()
        {
            dialoguePanelUI = Object.Instantiate(AssetLoader.LoadAsset(Prefabs.DiagloguePanelUI));
            dialogueLinesParent = dialoguePanelUI.GetComponentInChildren<VerticalLayoutGroup>().gameObject;
            dialoguePanelUI.SetActive(false);
            dialogueLineUI = AssetLoader.LoadAsset(Prefabs.DialogueLineUI);
            Instance = this;
        }

        public void StartDialogue(string nameOfSpeaker)
        {
            currentChoices.Clear();
            foreach (Transform child in dialogueLinesParent.transform)
            {
                Object.Destroy(child.gameObject);
            }
            ConverstationActive = true;
            WriteSpeakerName(nameOfSpeaker);
            WritePlayerDialogueLine(" ");
            ShowDialogue(true);
        }

        public void StopDialogue()
        {
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

        public void WritePlayerDialogueLine(string line)
        {
            var textGameObject = CreateDialogueLine(line);
            var textField = textGameObject.GetComponent<Text>();
            textField.text = "     " + textField.text;
        }

        public void WritePlayerChoiceLine(string line, Action onSelected)
        {
            var textGameObject = CreateDialogueLine(line);
            currentChoices.Add(textGameObject);

            var textField = textGameObject.GetComponent<Text>();
            textField.text = "     " + textField.text;
            textField.color = Color.yellow;

            var clickTrigger = textGameObject.GetComponent<EventTrigger>();
            var entry = new EventTrigger.Entry {eventID = EventTriggerType.PointerClick};
            entry.callback.AddListener(data =>
            {
                currentChoices.Remove(textGameObject);
                clickTrigger.triggers.Clear();
                DisableChoices();
                onSelected();
            });
            clickTrigger.triggers.Add(entry);
        }

        public void WriteNPCLine(string line)
        {
            CreateDialogueLine(line);
        }

        private void WriteSpeakerName(string line)
        {
            var textGameObject = CreateDialogueLine(line);
            var textField = textGameObject.GetComponent<Text>();
            textField.alignment = TextAnchor.UpperCenter;
        }

        private GameObject CreateDialogueLine(string line)
        {
            var lineGameObject = Object.Instantiate(dialogueLineUI);
            lineGameObject.transform.SetParent(dialogueLinesParent.transform);
            var text = lineGameObject.GetComponent<Text>();
            text.DOFade(0.0f, 0.0f);
            text.text = line;
            text.DOFade(1.0f, 4.0f);
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
                dialoguePanelUI.SetActive(true);
                dialogueLinesParent.GetComponent<RectTransform>().localScale = new Vector3(0f, 0f, 0f);
                dialogueLinesParent.GetComponent<RectTransform>().DOScale(new Vector3(1, 1, 1), 0.3f).SetEase(Ease.InOutCubic);
            }
            else
            {
                dialoguePanelUI.SetActive(false);
                //Closing the new conversation after it has started :(
                //dialogueLinesParent.GetComponent<RectTransform>().DOScale(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.InOutCubic).OnComplete(() => dialoguePanelUI.SetActive(false));
            }
        }
    }
}
