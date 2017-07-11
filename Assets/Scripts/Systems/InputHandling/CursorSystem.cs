using System.Collections.Generic;
using System.Linq;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Framework.Util;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Systems.InputHandling
{
    internal class CursorSystem : IFrameSystem, IEntityManager
    {
        private EntityStateSystem entitySystem;

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }

        public void OnFrame()
        {
            UpdateCursorState(StaticStates.Get<CursorState>());
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
                    cursorState.DebugEntity = nowSelectedEntity;
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
    }
}
