using System.Collections;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Systems.AI;
using JetBrains.Annotations;
using UnityEngine;
using Assets.Scripts.Systems.Drinks;
using Assets.Scripts.Util;
using Assets.Scripts.Systems.Cameras;
using Assets.Scripts.Systems.Bar;

namespace Assets.Scripts
{
    public class GameRunner : MonoBehaviour
    {
        [UsedImplicitly] public bool IsDebugOn;
        [UsedImplicitly] public bool SkipFirstDayFadein;
        [UsedImplicitly] public bool DisablePeople;
        [UsedImplicitly] public bool DisableTalkingToPlayer;

        private EntityStateSystem entitySystem;
        private bool tickingStarted;

        [UsedImplicitly]
        public void Awake()
        {
            GameSettings.IsDebugOn = Debug.isDebugBuild && IsDebugOn;
            GameSettings.SkipFirstDayFadein = SkipFirstDayFadein;
            GameSettings.DisableStory = DisablePeople;
            GameSettings.DisableTalkingToPlayer = DisableTalkingToPlayer;

            entitySystem = new EntityStateSystem();

            //Will want to start right at the end of "Day zero" (So we transition to first scripted day).
            StaticStates.Add(new TimeState(Constants.GameStartTime));
            StaticStates.Add(new CursorState(null, new SerializableVector3()));
            StaticStates.Add(new MoneyState(0));
            StaticStates.Add(new PlayerDecisions());

            //Debug
            entitySystem.AddSystem(new EntityTooltipSystem());

            //Init
            entitySystem.AddSystem(new WaypointSystem());
            entitySystem.AddSystem(new PositionSystem());
            entitySystem.AddSystem(new SpawningSystem());
            entitySystem.AddSystem(new RotationInitSystem());
            entitySystem.AddSystem(new InitVisualizersSystem());

            //Camera
            entitySystem.AddSystem(new CameraSystem());

            //Game
            entitySystem.AddSystem(new TimeSystem());
          
            entitySystem.AddSystem(new PathfindingSystem());            
            entitySystem.AddSystem(new HealthSystem());
            entitySystem.AddSystem(new DrinkMakingSystem());
            entitySystem.AddSystem(new ClickResponseSystem());
            entitySystem.AddSystem(new DialogueSystem());
            entitySystem.AddSystem(new HierarchyManipulationSystem()); //Must run before VisibleSlotSystem
            entitySystem.AddSystem(new VisibleSlotSystem());
            entitySystem.AddSystem(new GlassStackSystem());

            //NPC/AI
            entitySystem.AddSystem(new ActionManagerSystem());
            entitySystem.AddSystem(new DayDirectorSystem());
            //entitySystem.AddSystem(new PersonDescisionSystem());

            //Player
            entitySystem.AddSystem(new EntityInteractionSystem());
            entitySystem.AddSystem(new EntitySelectorSystem());

            entitySystem.Init();
        }

        [UsedImplicitly]
        public void Update()
        {
            if (!tickingStarted)
            {
                StartCoroutine(Ticker());
                tickingStarted = true;
            }
            entitySystem.Update();
        }

        private IEnumerator Ticker()
        {
            while (Application.isPlaying)
            {
                entitySystem.Tick();
                yield return new WaitForSeconds(Constants.TickPeriodInSeconds);
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}