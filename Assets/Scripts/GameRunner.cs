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
using Assets.Scripts.Systems.InputHandling;

namespace Assets.Scripts
{
    public class GameRunner : MonoBehaviour
    {
        public bool GameStarted;

        [UsedImplicitly] public bool IsDebugOn;
        [UsedImplicitly] public bool SkipFirstDayFadein;
        [UsedImplicitly] public bool DisableTutorial;

        private EntityStateSystem entitySystem;
        private bool tickingStarted;
        
        public static GameRunner Instance;

        [UsedImplicitly]
        public void Awake()
        {
            Instance = this;
        }

        public void StartGame()
        {
            GameSettings.IsDebugOn = Debug.isDebugBuild && IsDebugOn;
            GameSettings.SkipFirstDayFadein = SkipFirstDayFadein;
            GameSettings.DisableTutorial = DisableTutorial;

            entitySystem = new EntityStateSystem();

            //StaticStates
            StaticStates.Add(new DayPhaseState(DayPhase.Morning));
            StaticStates.Add(new TimeState(Constants.GameStartTime));
            StaticStates.Add(new CursorState(null, new SerializableVector3()));
            StaticStates.Add(new MoneyState(0));
            StaticStates.Add(new PlayerDecisionsState());

            //Debug
            entitySystem.AddSystem(new DebugControlsSystem());
            entitySystem.AddSystem(new EntityTooltipSystem());

            //Init
            entitySystem.AddSystem(new WaypointSystem());
            entitySystem.AddSystem(new TransformSystem());
            entitySystem.AddSystem(new SpawningSystem());
            entitySystem.AddSystem(new InitVisualizersSystem());

            //Camera
            entitySystem.AddSystem(new CameraSystem());

            //Game
            entitySystem.AddSystem(new TimeSystem());

            entitySystem.AddSystem(new PausingSystem());
            entitySystem.AddSystem(new PathfindingSystem());            
            entitySystem.AddSystem(new CharacterControllerSystem());
            entitySystem.AddSystem(new DrinkMakingSystem());
            entitySystem.AddSystem(new InputResponseSystem());
            entitySystem.AddSystem(new DialogueSystem());
            entitySystem.AddSystem(new HierarchyManipulationSystem()); //Must run before VisibleSlotSystem
            entitySystem.AddSystem(new VisibleSlotSystem());
            entitySystem.AddSystem(new ItemStackSystem());
            entitySystem.AddSystem(new BarQueueSystem());
            entitySystem.AddSystem(new BarEntitiesSystem());

            //NPC/AI
            entitySystem.AddSystem(new ActionManagerSystem());
            entitySystem.AddSystem(new DayDirectorSystem());

            //Input - Ordering of systems important here. 
            entitySystem.AddSystem(new CursorSystem());
            entitySystem.AddSystem(new InteractionSystem());
            entitySystem.AddSystem(new EntitySelectorSystem());

            //GameStart
            entitySystem.AddSystem(new GameSetupSystem());

            entitySystem.Init();
            GameStarted = true;
        }

        [UsedImplicitly]
        public void Update()
        {
            if (!GameStarted)
            {
                return;
            }

            if (!tickingStarted)
            {
                StartCoroutine(Ticker());
                tickingStarted = true;
            }
            entitySystem.Update();
        }

        [UsedImplicitly]
        public void FixedUpdate()
        {
            if (!GameStarted)
            {
                return;
            }

            entitySystem.FixedUpdate();
        }

        private IEnumerator Ticker()
        {
            while (Application.isPlaying)
            {
                entitySystem.Tick();
                yield return new WaitForSeconds(Constants.TickPeriodInSeconds);
            }
        }
    }
}