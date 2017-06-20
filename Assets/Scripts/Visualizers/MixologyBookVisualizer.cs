﻿using Assets.Framework.Entities;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.UI.Bar;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Visualizers
{
    [UsedImplicitly]
    class MixologyBookVisualizer : MonoBehaviour, IEntityVisualizer
    {
        private ActiveState activeState;
        private GameObject mixologyUI;

        public void OnStartRendering(Entity entity)
        {
            activeState = entity.GetState<ActiveState>();
        }

        public void OnFrame()
        {
            if (mixologyUI == null)
            {
                mixologyUI = Interface.Instance.MixologyBookUI;
                mixologyUI.GetComponent<MixologyBookUI>().CloseButton.onClick.AddListener(Close);
            }

            if (mixologyUI.activeSelf != activeState.IsActive)
            {
                mixologyUI.SetActive(activeState.IsActive);
            }
        }

        public void OnStopRendering(Entity entity)
        {
            //Do nothing
        }

        public void Close()
        {
            activeState.IsActive = false;
        }
    }
}
