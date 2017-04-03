using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Framework.Util;
using Assets.Scripts.Blueprints;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using Assets.Scripts.Util.NPC;
using UnityEngine;
using Assets.Scripts.States.Cameras;

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
            var player = SpawnPlayer(new Vector3(9.5f, 1.007366f, 0.6f));            
            SpawnCamera(new Vector3(12.07f, 15.9f, 0.0f), Quaternion.Euler(48, -90, 0), player);            
        }

        public void OnEndInit()
        {
            SpawnPeople(entitySystem);
            SpawnEntitiesFromBlueprints();
            CleanNestedBlueprints();
        }

        private List<Entity> SpawnEntitiesFromBlueprints(Entity parent, GameObject potentialBlueprintGameObject)
        {
            var entitiesSpawned = new List<Entity>();

            Entity entity = null;

            var possibleBlueprint = potentialBlueprintGameObject.GetComponent<IEntityBlueprint>();
            if (possibleBlueprint != null)
            {
                entity = entitySystem.CreateEntity(possibleBlueprint.EntityToSpawn(), false, false);
                var entityGameObject = entity.GameObject;
                entityGameObject.AddComponent<EntityIdComponent>().EntityId = entity.EntityId;

                if (parent != null)
                {
                    entityGameObject.transform.SetParent(parent.GameObject.transform, true);
                }

                entitiesSpawned.Add(entity);
            }

            var newParent = entity ?? parent;

            foreach (Transform child in potentialBlueprintGameObject.transform) {
                var childEntities = SpawnEntitiesFromBlueprints(newParent, child.gameObject);
                entitiesSpawned.AddRange(childEntities);
            }

            return entitiesSpawned;
        }

        private void SpawnEntitiesFromBlueprints()
        {
            var entitiesSpawned = new List<Entity>();

            var blueprints = GameObject.FindGameObjectWithTag("Blueprints");
            foreach (Transform blueprint in blueprints.transform)
            {
                var go = blueprint.gameObject;
                var spawned = SpawnEntitiesFromBlueprints(null, go);
                entitiesSpawned.AddRange(spawned);
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

        private void SpawnPeople(EntityStateSystem entityStateSystem)
        {
            var spawnPointPosition = GameObject.FindGameObjectWithTag("SpawnPoint").transform.position;
            NPCS.SpawnNpc(entityStateSystem, NPCS.Q, spawnPointPosition);
            NPCS.SpawnNpc(entityStateSystem, NPCS.Tolstoy, spawnPointPosition);
            NPCS.SpawnNpc(entityStateSystem, NPCS.Tolstoy, spawnPointPosition);
            NPCS.SpawnNpc(entityStateSystem, NPCS.Tolstoy, spawnPointPosition);
            NPCS.SpawnNpc(entityStateSystem, NPCS.Jannet, spawnPointPosition);
            NPCS.SpawnNpc(entityStateSystem, NPCS.McGraw, spawnPointPosition);
            NPCS.SpawnNpc(entityStateSystem, NPCS.Ellie, spawnPointPosition);
            NPCS.SpawnNpc(entityStateSystem, NPCS.Annon, spawnPointPosition);
            NPCS.SpawnNpc(entityStateSystem, NPCS.Annon, spawnPointPosition);
            NPCS.SpawnNpc(entityStateSystem, NPCS.Annon, spawnPointPosition);
            NPCS.SpawnNpc(entityStateSystem, NPCS.Annon, spawnPointPosition);

            var waypointPosition = GameObject.FindGameObjectWithTag("Waypoint").transform.position;
            NPCS.SpawnNpc(entityStateSystem, NPCS.HallwayWalker, waypointPosition);
            NPCS.SpawnNpc(entityStateSystem, NPCS.HallwayWalker, waypointPosition);
            NPCS.SpawnNpc(entityStateSystem, NPCS.HallwayWalker, waypointPosition);
            NPCS.SpawnNpc(entityStateSystem, NPCS.HallwayWalker, waypointPosition);
        }

        private Entity SpawnPlayer(Vector3 position)
        {
            var player = entitySystem.CreateEntity(new List<IState>
            {
                new PrefabState(Prefabs.Player),
                new InventoryState(),
                new VisibleSlotState(),
                new PositionState(position),
                new PathfindingState(null, null),
                new ActionBlackboardState(null),
                new NameState("You", 2.0f),
                new DialogueOutcomeState(),
                new ClothingState(ClothingTopType.BartenderTop, ClothingBottomType.BartenderBottom),
                new HairState(HairType.Bartender),
                new FaceState(FaceType.Bartender)
            });
            StaticStates.Add(new PlayerState(player));
            return player;
        }

        private void SpawnCamera(Vector3 position, Quaternion rotation, Entity player)
        {
            var camera = entitySystem.CreateEntity(new List<IState>
            {
                new RotationState(rotation),
                new PositionState(position),
                new PrefabState(Prefabs.Camera),
                new CounterState(),
                new CameraFollowState(player)
            }, false);
            StaticStates.Add(new CameraState(camera));
        }
    }
}
