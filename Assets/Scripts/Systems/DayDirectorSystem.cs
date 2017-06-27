using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions;
using Assets.Scripts.GameActions.Cutscenes;
using Assets.Scripts.GameActions.Inventory;
using Assets.Scripts.States;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using Random = UnityEngine.Random;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    class DayDirectorSystem : ITickEntitySystem, IEndInitEntitySystem
    {
        private TimeState time;
        private DayPhaseState dayPhase;
        private List<Entity> people;
        private List<Entity> hallwayWalkers;

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(IsPersonState) };
        }

        public void OnEndInit(List<Entity> allPeople)
        {
            dayPhase = StaticStates.Get<DayPhaseState>();
            time = StaticStates.Get<TimeState>();
            people = allPeople;
            hallwayWalkers = EntityQueries.GetNPCSWithName(allPeople, "Expendable");
            dayPhase.DayPhaseChangedTo += OnDayPhaseChanged;
            EventSystem.DayPhaseIncrementRequest += OnDayPhaseIncrementRequest;
        }

        private void OnDayPhaseIncrementRequest()
        {
            DoPhaseCleanup();
            StaticStates.Get<DayPhaseState>().IncrementDayPhase();
        }

        private void OnDayPhaseChanged(DayPhase newDayPhase)
        {
            switch (newDayPhase)
            {
                case DayPhase.Morning:
                    if(time.GameTime != Constants.GameStartTime)
                    {
                        time.GameTime.IncrementDay();
                        Interface.Instance.BlackFader.FadeToBlack(4.0f, string.Format("Day {0}", time.GameTime.GetDay()), () =>
                        {
                            DoPhaseSetup(newDayPhase);
                            if (time.GameTime.GetDay() == 2)
                            {
                                DayTwoMorning.Start(people);
                            }
                        });
                    }
                    break;
                case DayPhase.Open:
                    time.GameTime.SetTime(Constants.OpeningHour, 0);
                    Interface.Instance.BlackFader.FadeToBlack(4.0f, "Opening Time!", () =>
                    {
                        DoPhaseSetup(newDayPhase);
                        EventSystem.StartDrinkMakingEvent.Invoke();

                        switch (time.GameTime.GetDay())
                        {
                            case 1:
                                break;
                            case 2:
                                DayTwoOpen.Start(people);
                                break;
                        }
                    });
                    break;
                case DayPhase.Night:
                    Interface.Instance.BlackFader.FadeToBlack(5.0f, "Always some stragglers. Use the console near the door to turf them out.", () =>
                    {
                        DoPhaseSetup(newDayPhase);
                        EventSystem.EndDrinkMakingEvent.Invoke();
                        switch (time.GameTime.GetDay())
                        {
                            case 1:
                                DayOneNight.Start(people);
                                break;
                            case 2:
                                DayTwoNight.Start(people);
                                break;
                        }
                    });
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
                    OnDayPhaseIncrementRequest();
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
            ResetNPCs();
            ResetBarStateAndDialogues();
            SetLighting(newDayPhase);
        }

        private void ResetNPCs()
        {
            people.ForEach(person => ActionManagerSystem.Instance.TryClearActionsForEntity(person));
            people.ForEach(person => person.GetState<PersonAnimationState>().ResetAnimationState());
            people.ForEach(person => ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(person, new DestoryEntityInInventoryAction()));
            Locations.ResetPeopleToSpawnPoints(people);
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
