using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.Blueprints;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    class SpawningSystem : IInitSystem, IEntityManager
    {
        private EntityStateSystem entitySystem;

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }

        public void OnInit()
        {
            SpawnPlayer();
            SpawnPeople();
            SpawnEntitiesFromBlueprints();
        }

        private void SpawnPlayer()
        {
            var player = entitySystem.CreateEntity(new List<IState>
            {
                new PrefabState(Prefabs.Player),
                new InventoryState(),
                new VisibleSlotState()
            });
            StaticStates.Add(new PlayerState(player));
        }

        private void SpawnPeople()
        {
            for (var i = 0; i < 5; i++)
            {
                SpawnNPC(Prefabs.Person, new Vector3(12.28f, 0.0f, 11.21f));
            }
        }

        private void SpawnEntitiesFromBlueprints()
        {
            var allObjects = Object.FindObjectsOfType<GameObject>();
            foreach (var go in allObjects)
            {
                var possibleBlueprint = go.GetComponent<IEntityBlueprint>();
                if (possibleBlueprint != null)
                {
                    var entity = entitySystem.CreateEntity(possibleBlueprint.EntityToSpawn());
                    RemoveBlueprint(entity);
                    Object.Destroy(go);
                }
            }
            var spawnableRoot = GameObject.Find("Blueprints-DestroyedOnPlay");
            if (spawnableRoot != null)
            {
                Object.Destroy(spawnableRoot);
            }
        }

        private static void RemoveBlueprint(Entity entity)
        {
            var unneededBlueprint = entity.GameObject.GetComponent<IEntityBlueprint>() as MonoBehaviour;
            if (unneededBlueprint != null)
            {
                Object.Destroy(unneededBlueprint);
            }
        }

        private void SpawnNPC(string prefab, Vector3 position)
        {
            entitySystem.CreateEntity(new List<IState>
            {
                new PrefabState(prefab),
                new RandomWandererFlagState(),
                new PathfindingState(position),
                new InventoryState(),
                new VisibleSlotState()
            });
        }
    }
}
