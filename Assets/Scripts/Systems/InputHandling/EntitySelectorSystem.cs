using System.Collections.Generic;
using System.Linq;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Framework.Util;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Events;
using cakeslice;
using UnityEngine;

namespace Assets.Scripts.Systems.InputHandling
{
    internal class EntitySelectorSystem : IFrameSystem, IEntityManager
    {
        private EntityStateSystem entitySystem;
        private Entity lastEntitSelected;

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }

        public void OnFrame()
        {
            var cursorState = StaticStates.Get<CursorState>();
            UpdateCursorState(cursorState);
            HighlightEligibleUnderCursor(cursorState);
            HandleMouseClicks(cursorState);
        }

        private void UpdateCursorState(CursorState cursorState)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                var objectHit = hit.collider.gameObject;
                if (objectHit.GetEntityId() != EntityIdComponent.InvalidEntityId)
                {
                    var nowSelectedEntity = entitySystem.GetEntity(objectHit.GetEntityId());
                    if (nowSelectedEntity.HasState<InteractiveState>() && !nowSelectedEntity.GetState<InteractiveState>().CurrentlyInteractive)
                    {
                        return;
                    }
                    if (GetEntitiesInRange().Contains(nowSelectedEntity))
                    {
                        cursorState.SelectedEntity = nowSelectedEntity;
                    }
                }
                else
                {
                    cursorState.SelectedEntity = null;
                }
                cursorState.MousedOverPosition = hit.point;
            }
        }

        private static void HandleMouseClicks(CursorState cursorState)
        {
            var selectedEntity = cursorState.SelectedEntity;
            if (Input.GetMouseButtonDown(0))
            {
                if (selectedEntity.HasState<InteractiveState>() && selectedEntity.GetState<InteractiveState>().CurrentlyInteractive)
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

        private List<Entity> GetEntitiesInRange()
        {
            var playerEntity = StaticStates.Get<PlayerState>().Player;
            var playerPosition = playerEntity.GameObject.transform.position;
            var colliders = Physics.OverlapSphere(playerPosition, Constants.InteractRangeInMeters);
            if (!colliders.Any())
            {
                return null;
            }
            var entityIds = colliders.Select(collider => collider.gameObject.GetEntityIdRecursive());
            var entities = entityIds.Where(entityId => entityId != EntityIdComponent.InvalidEntityId).Select(entityId => entitySystem.GetEntity(entityId)).ToList();
            return entities;
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
