using System;
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
                    Recursive.ApplyActionRecursively(objectHit.GetEntityObject().transform, AddOutline);

                    StaticStates.Get<CursorState>().SelectedEntity = entitySystem.GetEntity(objectHit.GetEntityId());
                }
                else
                {
                    StaticStates.Get<CursorState>().SelectedEntity = null;
                }
                StaticStates.Get<CursorState>().MousedOverPosition = hit.point;
            }
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
