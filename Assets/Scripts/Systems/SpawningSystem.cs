using System.Collections.Generic;
using System.Xml;
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
        private const int NonNamedNpcs = 0;

        private EntityStateSystem entitySystem;

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }

        public void OnInit()
        {
            SpawnPlayer(new Vector3(9.5f, 1.007366f, 0.6f));
            SpawnPeople();
            SpawnEntitiesFromBlueprints();
        }

        public void OnEndInit()
        {
            CleanNestedBlueprints();
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

        private void CleanNestedBlueprints()
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
                var isEntityRoot = go.GetComponent<EntityIdComponent>() as MonoBehaviour;
                if (possibleBlueprint != null && !isEntityRoot)
                {
                    Object.Destroy(possibleBlueprint.gameObject);
                }
            }
        }

        private void SpawnPeople()
        {
            SpawnNpc(Color.red, "Q");
            SpawnNpc(Color.blue, "Tolstoy");
            SpawnNpc(Color.yellow, "Jannet");
            SpawnNpc(Color.cyan, "McGraw");
            SpawnNpc(Color.green, "Ellie");

            for (var i = 0; i < NonNamedNpcs; i++)
            {
                SpawnNpc(Color.white);
            }
        }

        private void SpawnPlayer(Vector3 position)
        {
            var player = entitySystem.CreateEntity(new List<IState>
            {
                new PrefabState(Prefabs.Player),
                new HierarchyState(),
                new VisibleSlotState(),
                new PositionState(position),
                new PathfindingState(null),
                new ActionBlackboardState(null),
                new NameState("You")
            });
            StaticStates.Add(new PlayerState(player));
        }

        private void SpawnNpc(Color color, string name = "Expendable")
        {
            entitySystem.CreateEntity(new List<IState>
            {
                new ActionBlackboardState(null),
                new PrefabState(Prefabs.Person),
                new NameState(name),
                new PositionState(new Vector3(5.63f, 0.0f, 16.49f)),
                new PathfindingState(null),
                new HierarchyState(),
                new VisibleSlotState(),
                new PersonState(),
                new ColorState(color),
                new MoodState(Mood.Happy),
                new DialogueOutcomeState()
            });
        }
    }
}
