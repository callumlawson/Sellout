using System;
using Assets.Framework.Entities;
using Assets.Framework.States;
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

        public void OnStartRendering(Entity entity)
        {
            positionState = entity.GetState<PositionState>();
            moodBubble = Instantiate(Resources.Load(Prefabs.MoodBubbleUI) as GameObject);
            moodBubble.transform.SetParent(interfaceMonobehaviour.gameObject.transform);
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

            moodBubble.SetActive(true);
            var tweenSequence = DOTween.Sequence();
            tweenSequence.Append(imageComponent.DOFade(1.0f, 1.0f));
            tweenSequence.AppendInterval(2.5f);
            tweenSequence.Append(imageComponent.DOFade(0.0f, 1.0f));
            tweenSequence.AppendCallback(() => moodBubble.SetActive(false));
        }

        public void OnFrame()
        {
            moodBubbleRectTransform.anchoredPosition = interfaceMonobehaviour.GetComponent<Canvas>().WorldToCanvas(positionState.Position + offset);
        }

        public void OnStopRendering(Entity entity)
        {
            Destroy(moodBubble);
        }
    }
}