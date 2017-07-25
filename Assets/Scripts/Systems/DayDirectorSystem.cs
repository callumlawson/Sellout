using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions;
using Assets.Scripts.GameActions.Cutscenes;
using Assets.Scripts.States;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using Random = UnityEngine.Random;
using UnityEngine;
using Assets.Scripts.States.AI;
using Assets.Scripts.Util.NPC;
using Assets.Scripts.Visualizers;

namespace Assets.Scripts.Systems
{
    class DayDirectorSystem : ITickEntitySystem, IEndInitEntitySystem
    {
        private TimeState time;
        private DayPhaseState dayPhase;
        private List<Entity> people;
        private List<Entity> hallwayWalkers;

        public static DayDirectorSystem Instance;

        private bool doingPhaseChange = false;

        public DayDirectorSystem()
        {
            Instance = this;
        }

        public void RequestIncrementDayPhase()
        {
            if (doingPhaseChange)
            {
                Debug.Log("Unable to increment day phase as we're already in the middle of incrementing the day phase.");
                return;
            }

            doingPhaseChange = true;

            EntityStateSystem.Instance.Pause();

            var nextDayPhase = StaticStates.Get<DayPhaseState>().GetNextDayPhase();

            Interface.Instance.BlackFader.FadeToBlack(4.0f, GetFadeTitle(nextDayPhase), () =>
            {
                DoPhaseCleanup();

                DoPhaseSetup(nextDayPhase);

                StaticStates.Get<DayPhaseState>().IncrementDayPhase();

                EntityStateSystem.Instance.Resume();

                doingPhaseChange = false;
            });
        }

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(IsPersonState) };
        }

        public void OnEndInit(List<Entity> allPeople)
        {
            dayPhase = StaticStates.Get<DayPhaseState>();
            time = StaticStates.Get<TimeState>();
            people = allPeople;
            hallwayWalkers = EntityQueries.GetNPCSWithName(allPeople, NPCName.Expendable);
            dayPhase.DayPhaseChangedTo += OnDayPhaseChanged;
        }

        private string GetFadeTitle(DayPhase newDayPhase)
        {
            switch (newDayPhase)
            {
                case DayPhase.Morning:
                    return string.Format("Day {0}", time.GameTime.GetDay());
                case DayPhase.Open:
                    return "Opening Time!";
                case DayPhase.Night:
                    return "Always some stragglers. Use the console near the door to turf them out.";
                default:
                    throw new ArgumentOutOfRangeException("newDayPhase", newDayPhase, null);
            }
        }

        private void OnDayPhaseChanged(DayPhase newDayPhase)
        {
            switch (newDayPhase)
            {
                case DayPhase.Morning:
                    if(time.GameTime != Constants.GameStartTime)
                    {
                        time.GameTime.IncrementDay();
                        switch (time.GameTime.GetDay())
                        {
                            case 1:
                                break;
                            case 2:
                                DayTwoMorning.Start(people);
                                break;
                            case 4:
                                WelcomeSignControllerVisualizer.Instance.SetWelcomeSignActive(false);
                                break;
                        }
                    }
                    break;
                case DayPhase.Open:
                    time.GameTime.SetTime(Constants.OpeningHour, 0);
                    EventSystem.StartDrinkMakingEvent.Invoke();

                    switch (time.GameTime.GetDay())
                    {
                        case 1:
                            break;
                        case 2:
                            DayTwoOpen.Start(people);
                            break;
                    }
                    break;
                case DayPhase.Night:
                    EventSystem.EndDrinkMakingEvent.Invoke();
                    switch (time.GameTime.GetDay())
                    {
                        case 1:
                            DayOneNight.Start(people);
                            break;
                        case 2:
                            DayTwoNight.Start(people);
                            break;
                        case 3:
                            PartyScene.Start(people);
                            break;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("newDayPhase", newDayPhase, null);
            }
        }

        public void Tick(List<Entity> matchingEntities)
        {
            var currentTime = time.GameTime;

            if (currentTime == Constants.GameStartTime)
            {
                PerformStartGameActions();
                time.GameTime.IncrementMinute();
            }

            if (dayPhase.CurrentDayPhase == DayPhase.Open)
            {
                if (currentTime.GetHour() == Constants.ClosingHour)
                {
                    RequestIncrementDayPhase();
                }
            }

            foreach (var walker in hallwayWalkers)
            {
                if (ActionManagerSystem.Instance.IsEntityIdle(walker))
                {
                    if (dayPhase.CurrentDayPhase != DayPhase.Night || Random.value > 0.8)
                    {
                        ActionManagerSystem.Instance.QueueAction(walker, CommonActions.WalkToWaypoint());
                    }
                }
            }
        }

        private void PerformStartGameActions()
        {
            Locations.ResetPeopleToSpawnPoints(people);
            if (!GameSettings.SkipFirstDayFadein)
            {
                Interface.Instance.BlackFader.FadeToBlack(4.0f, "Day 1", null, false);
            }

            DayOneMorning.Start(people);
        }

        private void DoPhaseCleanup()
        {
            ResetNPCs();
            ResetBarStateAndDialogues();
            WaypointSystem.Instance.ClearAllWaypoints();
        }

        private void DoPhaseSetup(DayPhase newDayPhase)
        {
            SetLighting(newDayPhase);
        }

        private void ResetNPCs()
        {
            people.ForEach(person => ActionManagerSystem.Instance.TryCancelThenHardClearActions(person));
            people.ForEach(person => person.GetState<PersonAnimationState>().ResetAnimationState());
            people.ForEach(person =>
            {
                if (person.HasState<LifecycleState>())
                {
                    person.GetState<LifecycleState>().status = LifecycleState.LifecycleStatus.Offscreen;
                }
            });
            people.ForEach(RemoveInventoryItem);
            Locations.ResetPeopleToSpawnPoints(people);
        }
        
        private static void RemoveInventoryItem(Entity entity)
        {
            if(entity.HasState<IsPlayerState>())
            {
                return;
            }
            var inventoryItem = entity.GetState<InventoryState>().Child;
            if (inventoryItem != null)
            {
                entity.GetState<InventoryState>().RemoveChild();
                EntityStateSystem.Instance.RemoveEntity(inventoryItem);
            }
        }

        private void ResetBarStateAndDialogues()
        {
            DialogueSystem.Instance.StopDialogue();
            EventSystem.EndDrinkOrderEvent.Invoke();
        }

        private static void SetLighting(DayPhase newDayPhase)
        {
            LightControllerVisualizer.Instance.SetLighting(newDayPhase);
        }
    }
}
