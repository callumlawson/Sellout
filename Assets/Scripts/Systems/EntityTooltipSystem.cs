using System;
using System.Text;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Systems
{
    class EntityTooltipSystem : IFrameSystem, IInitSystem
    {
        private const float TooltipTime = 0.2f;
        private float hoverTime;
        private Entity lastSelectedEntity;

        private GameObject tooltipWindow;
        private GameObject tooltipRoot;

        public void OnInit()
        {
            tooltipWindow = InterfaceComponents.Instance.TooltipWindow;
            tooltipRoot = InterfaceComponents.Instance.TooltipRoot;
            tooltipWindow.SetActive(false);
        }

        public void OnFrame()
        {
            var selectedEntity = StaticStates.Get<SelectedState>().SelectedEntity;

            UpdateHoverTime(selectedEntity);
            CleanPreviousTooltips();

            tooltipRoot.GetComponent<RectTransform>().transform.position = Input.mousePosition;

            if (hoverTime > TooltipTime && selectedEntity != null)
            {
                var tooltip = UnityEngine.Object.Instantiate(tooltipWindow);
                tooltip.GetComponent<RectTransform>().SetParent(tooltipRoot.transform);
                var textComponent = tooltip.GetComponentInChildren<Text>();
                textComponent.text = TooltipMessage(selectedEntity);

                MatchWidths();

                foreach (Transform child in tooltipRoot.transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
        }

        private static string TooltipMessage(Entity entity)
        {
            var message = new StringBuilder();
            message.Append(string.Format("Entity ID: {0}", entity.EntityId));
            foreach (var state in entity.DebugStates)
            {
                message.Append(Environment.NewLine);
                message.Append(state);
            }
            return message.ToString();
        }

        private void UpdateHoverTime(Entity entity)
        {
            if (Equals(entity, lastSelectedEntity))
            {
                hoverTime += Time.deltaTime;
            }
            else
            {
                hoverTime = 0;
            }
            lastSelectedEntity = entity;
        }

        private void CleanPreviousTooltips()
        {
            foreach (Transform child in tooltipRoot.transform)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }
        }

        private void MatchWidths()
        {
            var largestWidth = 0.0f;
            foreach (Transform child in tooltipRoot.transform)
            {
                var width = child.GetComponent<RectTransform>().rect.width;
                if (width > largestWidth)
                {
                    largestWidth = width;
                }
            }

            foreach (Transform child in tooltipRoot.transform)
            {
                child.GetComponent<LayoutElement>().minWidth = largestWidth;
            }
        }

    }
}
