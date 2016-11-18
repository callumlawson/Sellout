using System.Collections.Generic;
using Assets.Framework.States;
using Assets.Framework.Systems;
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
            for (var i = 0; i < 5; i++)
            {
                Spawn(Prefabs.Person, new Vector3(12.28f, 0.0f, 11.21f));
            }
        }

        private void Spawn(string prefab, Vector3 position)
        {
            entitySystem.CreateEntity(new List<IState>
            {
                new PrefabState(prefab),
                new RandomWandererFlagState(),
                new PathfindingState(position)
            });
        }
    }
}
