using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Framework.Util;
using Assets.Scripts.States;
using Assets.Scripts.Util;
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
                var selectedEntity = StaticStates.Get<SelectedState>().SelectedEntity;

                if (selectedEntity != null)
                {
                    Recursive.SetLayerRecursively(selectedEntity.GameObject.transform, LayerMask.NameToLayer("Default"));
                }

                if (objectHit.GetEntityId() != EntityIdComponent.InvalidEntityId)
                {
                    Recursive.SetLayerRecursively(objectHit.GetEntityObject().transform, LayerMask.NameToLayer("Outline"));
                    StaticStates.Get<SelectedState>().SelectedEntity = entitySystem.GetEntity(objectHit.GetEntityId());
                }
                else
                {
                    StaticStates.Get<SelectedState>().SelectedEntity = null;
                }
            }
        }
    }
}
