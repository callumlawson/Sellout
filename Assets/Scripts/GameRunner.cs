using System.Collections;
using System.Collections.Generic;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameRunner : MonoBehaviour
    {
        private EntityStateSystem entitySystem;

        [UsedImplicitly]
        public void Awake()
        {
            entitySystem = new EntityStateSystem();

            StaticStates.Add(new SelectedState(gameObject));

            entitySystem.AddSystem(new HealthSystem());

            entitySystem.CreateEntity(new List<IState>{new HealthState(100.0f)});

            entitySystem.Init();
            StartCoroutine(Ticker());
        }

        [UsedImplicitly]
        public void Update()
        {
            entitySystem.Update();
        }

        private IEnumerator Ticker()
        {
            while (true)
            {
                entitySystem.Tick();
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}