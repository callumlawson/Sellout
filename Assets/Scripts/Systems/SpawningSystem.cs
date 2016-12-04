using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Framework.Util;
using Assets.Scripts.Blueprints;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    class SpawningSystem : IInitSystem, IEndInitSystem, IEntityManager
    {
        private EntityStateSystem entitySystem;

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }

        public void OnInit()
        {
            SpawnPlayer(new Vector3(-3.5f, 0, -15));
            SpawnPeople();
            SpawnEntitiesFromBlueprints();
        }

        public void OnEndInit()
        {
            CleanUpBlueprints();
        }

        private void SpawnEntitiesFromBlueprints()
        {
            var entitiesSpawned = new List<Entity>();
            var allObjects = Object.FindObjectsOfType<GameObject>();
            foreach (var go in allObjects)
            {
                var possibleBlueprint = go.GetComponent<IEntityBlueprint>();
                if (possibleBlueprint != null)
                {
                    var entity = entitySystem.CreateEntity(possibleBlueprint.EntityToSpawn(), false, false);
                    go.AddComponent<EntityIdComponent>().EntityId = entity.EntityId;
                    entitiesSpawned.Add(entity);
                }
            }
            foreach (var entity in entitiesSpawned)
            {
                entitySystem.EntityAdded(entity);
            }
        }

        private void CleanUpBlueprints()
        {
            var spawnableRoot = GameObject.Find("Blueprints-DestroyedOnPlay");
            if (spawnableRoot != null)
            {
                Object.Destroy(spawnableRoot);
            }

            var allObjects = Object.FindObjectsOfType<GameObject>();
            foreach (var go in allObjects)
            {
                var possibleBlueprint = go.GetComponent<IEntityBlueprint>() as MonoBehaviour;
                if (possibleBlueprint != null)
                {
                    Object.Destroy(possibleBlueprint);
                }
            }
        }

        private void SpawnPeople()
        {
            for (var i = 0; i < 5; i++)
            {
                SpawnNpc(Prefabs.Person, new Vector3(5f, 0.0f, 14f));
            }
        }

        private void SpawnPlayer(Vector3 position)
        {
            var player = entitySystem.CreateEntity(new List<IState>
            {
                new PrefabState(Prefabs.Player),
                new InventoryState(),
                new VisibleSlotState(),
                new PositionState(position),
                new PathfindingState(null),
                new ActionBlackboardState(null),
            });
            StaticStates.Add(new PlayerState(player));
        }

        private void SpawnNpc(string prefab, Vector3 position)
        {
            entitySystem.CreateEntity(new List<IState>
            {
                new PrefabState(prefab),
                new PositionState(position),
                new PathfindingState(null),
                new ActionBlackboardState(null),
                new InventoryState(),
                new VisibleSlotState(),
                new PersonState()
            });
        }
    }
}
