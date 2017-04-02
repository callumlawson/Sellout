﻿using System;
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

namespace Assets.Scripts
{
    public class GameRunner : MonoBehaviour
    {
        [UsedImplicitly] public bool IsDebugOn;

        private EntityStateSystem entitySystem;
        private bool tickingStarted;

        [UsedImplicitly]
        public void Awake()
        {
            GameSettings.IsDebugOn = Debug.isDebugBuild && IsDebugOn;

            entitySystem = new EntityStateSystem();

            //Will want to start right at the end of "Day zero" (So we transition to first scripted day).
            StaticStates.Add(new TimeState(new DateTime(2050, 1, 1, 20, 50, 0)));
            StaticStates.Add(new CursorState(null, new SerializableVector3()));
            StaticStates.Add(new MoneyState(0));

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