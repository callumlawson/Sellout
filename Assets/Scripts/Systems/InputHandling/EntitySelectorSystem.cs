using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Events;
using cakeslice;
using UnityEngine;

namespace Assets.Scripts.Systems.InputHandling
{
    internal class EntitySelectorSystem : IFrameSystem
    {
        private Entity lastEntitSelected;

        public void OnFrame()
        {
            var cursorState = StaticStates.Get<CursorState>();
            HighlightEligibleUnderCursor(cursorState);
            HandleMouseClicks(cursorState);
        }

        private static void HandleMouseClicks(CursorState cursorState)
        {
            var selectedEntity = cursorState.SelectedEntity;
            if (Input.GetMouseButtonDown(0))
            {
                if (selectedEntity == null)
                {
                    EventSystem.OnClickedEvent(new ClickEvent(null, cursorState.MousedOverPosition, 0));
                }
                else if (!selectedEntity.HasState<InteractiveState>() || selectedEntity.GetState<InteractiveState>().CurrentlyInteractive)
                {
                    EventSystem.OnClickedEvent(new ClickEvent(selectedEntity, cursorState.MousedOverPosition, 0));
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                EventSystem.OnClickedEvent(new ClickEvent(selectedEntity, cursorState.MousedOverPosition, 1));
            }
        }

        private void HighlightEligibleUnderCursor(CursorState cursorState)
        {
            var selectedEntity = cursorState.SelectedEntity;
            if (lastEntitSelected != null)
            {
                Recursive.ApplyActionRecursively(lastEntitSelected.GameObject.transform, ResetOutline);
            }
            if (selectedEntity != null)
            {
                Recursive.ApplyActionRecursively(selectedEntity.GameObject.transform, AddOutline);
            }
            lastEntitSelected = selectedEntity;
        }

        private static void ResetOutline(Transform transform)
        {
            var objectHit = transform.gameObject;
            var outline = objectHit.GetComponent<Outline>();
            if (outline)
            {
                outline.enabled = false;
            }
        }

        private static void AddOutline(Transform transform)
        {
            var objectHit = transform.gameObject;
            if (!objectHit.GetComponent<Renderer>() || objectHit.GetComponent<TextMesh>())
            {
                return;
            }
            var outline = objectHit.GetComponent<Outline>() ?? objectHit.AddComponent<Outline>();
            outline.enabled = true;
        }
    }
}
