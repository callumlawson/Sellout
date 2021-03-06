﻿using System;
using Assets.Framework.Entities;
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
    [UsedImplicitly]
    class MoodStateVisualizer : MonoBehaviour, IEntityVisualizer
    {
        private readonly Interface interfaceMonobehaviour = Interface.Instance;

        [UsedImplicitly] public Sprite Angry;
        [UsedImplicitly] public Sprite Happy;

        private PositionState positionState;
        private GameObject moodBubble;
        private RectTransform moodBubbleRectTransform;
        private readonly Vector3 offset = new Vector3(0.0f, 2.8f, 0.0f);

        private Sequence moodTweenSequence;

        public void OnStartRendering(Entity entity)
        {
            positionState = entity.GetState<PositionState>();
            moodBubble = Instantiate(AssetLoader.LoadAsset(Prefabs.MoodBubbleUI));
            moodBubble.transform.SetParent(interfaceMonobehaviour.DyanmicUIRoot.transform);
            moodBubble.SetActive(false);
            moodBubbleRectTransform = moodBubble.GetComponent<RectTransform>();
            entity.GetState<MoodState>().MoodEvent += OnMoodUpdated;
        }

        private void OnMoodUpdated(Mood mood)
        {
            Sprite sprite;
            switch (mood)
            {
                case Mood.Angry:
                    sprite = Angry;
                    break;
                case Mood.Happy:
                    sprite = Happy;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("mood", mood, "Could not find image for mood");
            }
            var imageComponent = moodBubble.GetComponent<Image>();
            imageComponent.overrideSprite = sprite;

            if (moodTweenSequence != null && !moodTweenSequence.IsComplete())
            {
                moodTweenSequence.Kill();
            }
            moodBubble.SetActive(true);
            imageComponent.DOFade(0.0f, -1f);
            moodTweenSequence = DOTween.Sequence()
                .Append(imageComponent.DOFade(1.0f, 0.7f))
                .AppendInterval(1.8f)
                .Append(imageComponent.DOFade(0.0f, 0.7f))
                .AppendCallback(() => moodBubble.SetActive(false));
        }

        public void OnFrame()
        {
            moodBubbleRectTransform.anchoredPosition =
                interfaceMonobehaviour.GetComponent<Canvas>().WorldToCanvas(positionState.Position + offset);
        }

        public void OnStopRendering(Entity entity)
        {
            Destroy(moodBubble);
        }
    }
}