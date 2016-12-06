using System;
using System.Collections;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Systems.AI;
using JetBrains.Annotations;
using UnityEngine;
using Assets.Scripts.Systems.Drinks;

namespace Assets.Scripts
{
    public class GameRunner : MonoBehaviour
    {
        private const float TickPeriodInSeconds = 0.4f;
        private EntityStateSystem entitySystem;

        [UsedImplicitly]
        public void Start()
        {
            entitySystem = new EntityStateSystem();

            StaticStates.Add(new SelectedState(null));
            StaticStates.Add(new TimeState(new DateTime(2337, 1, 1, 9, 0, 0)));

            //Debug
            entitySystem.AddSystem(new EntityTooltipSystem());

            //Init
            entitySystem.AddSystem(new WaypointSystem());
            entitySystem.AddSystem(new PositionSystem());
            entitySystem.AddSystem(new SpawningSystem());
            entitySystem.AddSystem(new RotationInitSystem());
            entitySystem.AddSystem(new InitVisualizersSystem());

            //Game
            entitySystem.AddSystem(new TimeSystem());
            entitySystem.AddSystem(new PathfindingSystem());            
            entitySystem.AddSystem(new HealthSystem());
            entitySystem.AddSystem(new DrinkMakingSystem());
            entitySystem.AddSystem(new InventoryExchangeSystem());
            entitySystem.AddSystem(new VisibleSlotSystem());
            entitySystem.AddSystem(new CharacterResponseSystem());
            entitySystem.AddSystem(new DialogueSystem());

            //NPC/AI
            entitySystem.AddSystem(new ActionManagerSystem());
            entitySystem.AddSystem(new PersonDescisionSystem());

            //OLD AI SYSTEM
            //entitySystem.AddSystem(new GoalDecisionSystem());
            //entitySystem.AddSystem(new SitGoalSystem());
            //entitySystem.AddSystem(new PayForGoalSystem());
            //entitySystem.AddSystem(new WanderGoalSystem());

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