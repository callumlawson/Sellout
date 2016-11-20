using System.Collections;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Systems.AI;
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

            //Debug
            entitySystem.AddSystem(new EntityTooltipSystem());

            //Init
            entitySystem.AddSystem(new WaypointInitSystem());
            entitySystem.AddSystem(new PositionInitSystem());
            entitySystem.AddSystem(new SpawningSystem());

            //Game
            entitySystem.AddSystem(new PathfindingSystem());            
            entitySystem.AddSystem(new HealthSystem());
            entitySystem.AddSystem(new DrinkMakingSystem());
            entitySystem.AddSystem(new InventoryExchangeSystem());
            entitySystem.AddSystem(new VisibleSlotSystem());

            //NPC/AI
            entitySystem.AddSystem(new GoalDecisionSystem());
            entitySystem.AddSystem(new SitGoalSystem());
            entitySystem.AddSystem(new PayForGoalSystem());
            entitySystem.AddSystem(new WanderGoalSystem());

            //Player
            entitySystem.AddSystem(new PlayerInventoryInteractionSystem());
            entitySystem.AddSystem(new EntityInteractionSystem());
            entitySystem.AddSystem(new EntitySelectorSystem());

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