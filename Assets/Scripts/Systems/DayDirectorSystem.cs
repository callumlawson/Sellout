using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions.Cutscenes;
using Assets.Scripts.States;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using DG.Tweening;

namespace Assets.Scripts.Systems
{
    class DayDirectorSystem : ITickEntitySystem, IEndInitEntitySystem
    {
        private TimeState time;
        private GameTime lastTime;
        private DayPhaseState dayPhase;
        private List<Entity> people;

        private bool doneFirstDayFadeIn;
        private readonly List<Day> inGameDays = new List<Day> ();

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(PersonState) };
        }

        public void OnEndInit(List<Entity> matchingEntities)
        {
            people = matchingEntities;
            dayPhase = StaticStates.Get<DayPhaseState>();
            dayPhase.DayPhaseChangedTo += OnDayPhaseChanged;
            time = StaticStates.Get<TimeState>();
            inGameDays.Add(new FirstDay(matchingEntities));
            inGameDays.Add(new SecondDay(matchingEntities));
        }

        private void OnDayPhaseChanged(DayPhase newDayPhase)
        {
            switch (newDayPhase)
            {
                case DayPhase.Morning:
                    SetLighting(newDayPhase);
                    ResetNPCs();
                    break;
                case DayPhase.Open:
                    time.gameTime.SetTime(Constants.OpeningHour, 0);
                    Interface.Instance.BlackFader.FadeToBlack(4.0f, "Opening Time", () =>
                    {
                        ResetNPCs();
                        EventSystem.StartDrinkMakingEvent.Invoke();
                        SetLighting(newDayPhase);
                    });
                    break;
                case DayPhase.Night:
                    Interface.Instance.BlackFader.FadeToBlack(4.0f, "Always some stragglers. Use the console near the door to turf them out.", () =>
                    {
                        ResetNPCs();
                        EventSystem.EndDrinkMakingEvent.Invoke();
                        SetLighting(newDayPhase);
                        DayOneNight.Start(people); //TODO support more than one day
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException("newDayPhase", newDayPhase, null);
            }
        }

        private void ResetNPCs()
        {
            people.ForEach(person => ActionManagerSystem.Instance.TryClearActionsForEntity(person));
            people.ForEach(person => person.GetState<PersonAnimationState>().ResetAnimationState());
            Locations.ResetPeopleToSpawnPoints(people);
        }

        private static void SetLighting(DayPhase newDayPhase)
        {
            LightControllerVisualizer.Instance.SetLighting(newDayPhase);
        }

        public void Tick(List<Entity> matchingEntities)
        {
            var currentTime = time.gameTime;
            if (currentTime == Constants.GameStartTime && !GameSettings.SkipFirstDayFadein && !doneFirstDayFadeIn)
            {
                StaticStates.Get<TimeState>().TriggerDayTransition.Invoke("Day 1", false, true);
                doneFirstDayFadeIn = true;
            }

            if (time.GameEnded)
            {
                return;
            }

            if (dayPhase.CurrentDayPhase == DayPhase.Open)
            {
                if (currentTime.GetHour() == Constants.ClosingHour)
                {
                    StaticStates.Get<DayPhaseState>().IncrementDayPhase();
                }
            }
        }

        /*
        private void UpdateDay(GameTime currentTime, List<Entity> matchingEntities)
        {
            if (currentTime.GetHour() >= Constants.ClosingHour)
            {
                currentTime.IncrementDay();
                TriggerEndOfDayAfterDelay(currentTime, matchingEntities);
                
                lastTime = currentTime.GetCopy();

                if (currentTime.GetDay() - 1 < inGameDays.Count)
                {
                    StaticStates.Get<TimeState>().TriggerDayTransition.Invoke(string.Format("Day {0}", currentTime.GetDay()), true, true);
                }

                if (currentTime.GetDay() - 1 >= inGameDays.Count)
                {
                    StaticStates.Get<TimeState>().TriggerEndOfGame.Invoke();
                    StaticStates.Get<TimeState>().GameEnded = true;
                    UnityEngine.Debug.Log("Reached the end of the last day!");
                    return inGameDays[0];
                }
            }

            return inGameDays[currentTime.GetDay() - 1];
        }

        private void TriggerEndOfDayAfterDelay(GameTime currentTime, List<Entity> matchingEntities)
        {
            var dayToEnd = inGameDays[currentTime.GetDay() - 1];
            DOTween.Sequence().SetDelay(3.0f).OnComplete(() => dayToEnd.OnEndOfDay(matchingEntities)); //HAX
        }
        */
    }
}
