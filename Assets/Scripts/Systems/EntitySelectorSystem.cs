using System.Collections.Generic;
using System.Linq;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Framework.Util;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using cakeslice;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    class EntitySelectorSystem : IFrameSystem, IEntityManager
    {
        private EntityStateSystem entitySystem;

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }

        public void OnFrame()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                var objectHit = hit.collider.gameObject;
                var selectedEntity = StaticStates.Get<CursorState>().SelectedEntity;

                if (selectedEntity != null)
                {
                    Recursive.ApplyActionRecursively(selectedEntity.GameObject.transform, ResetOutline);
                }

                if (objectHit.GetEntityId() != EntityIdComponent.InvalidEntityId)
                {
                    var nowSelectedEntity = entitySystem.GetEntity(objectHit.GetEntityId());
                    if (nowSelectedEntity.HasState<InteractiveState>() &&
                        !nowSelectedEntity.GetState<InteractiveState>().CurrentlyInteractive)
                    {
                        return;
                    }
                    if (GetEntitiesInRange().Contains(nowSelectedEntity))
                    {
                        Recursive.ApplyActionRecursively(objectHit.GetEntityObject().transform, transform => AddOutline(transform, true));
                        StaticStates.Get<CursorState>().SelectedEntity = nowSelectedEntity;
                    }
                    else
                    {
                        Recursive.ApplyActionRecursively(objectHit.GetEntityObject().transform, transform => AddOutline(transform, false));
                    }
                }
                else
                {
                    StaticStates.Get<CursorState>().SelectedEntity = null;
                }
                StaticStates.Get<CursorState>().MousedOverPosition = hit.point;
            }
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

        private static void AddOutline(Transform transform, bool isPositive)
        {
            var objectHit = transform.gameObject;
            if (!objectHit.GetComponent<Renderer>() || objectHit.GetComponent<TextMesh>())
            {
                return;
            }
            var outline = objectHit.GetComponent<Outline>() ?? objectHit.AddComponent<Outline>();
            outline.enabled = true;
            outline.color = isPositive ? 0 : 1;
        }
    }
}
