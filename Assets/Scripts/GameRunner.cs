using System.Collections;
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
        private const float TickPeriodInSeconds = 1f;
        private EntityStateSystem entitySystem;

        [UsedImplicitly]
        public void Start()
        {
            entitySystem = new EntityStateSystem();

            StaticStates.Add(new SelectedState(null));

            entitySystem.AddSystem(new EntitySelectorSystem());
            entitySystem.AddSystem(new EntityTooltipSystem());
            entitySystem.AddSystem(new SpawningSystem());
            entitySystem.AddSystem(new PathfindingSystem());
            entitySystem.AddSystem(new RandomWanderSystem());
            entitySystem.AddSystem(new HealthSystem());

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
                yield return new WaitForSeconds(TickPeriodInSeconds);
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}