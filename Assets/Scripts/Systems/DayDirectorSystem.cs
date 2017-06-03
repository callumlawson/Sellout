using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using DG.Tweening;

namespace Assets.Scripts.Systems
{
    class DayDirectorSystem : ITickEntitySystem, IEndInitEntitySystem
    {
        private TimeState time;
        private Day currentDay;
        private DateTime lastTime;
        private DayPhaseState dayPhase;
        private List<Entity> initPeople;

        private bool DoneFirstDayFadeIn;
        private readonly List<Day> inGameDays = new List<Day> ();

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(PersonState) };
        }

        public void OnEndInit(List<Entity> matchingEntities)
        {
            initPeople = matchingEntities;
            dayPhase = StaticStates.Get<DayPhaseState>();
            dayPhase.DayPhaseChangedTo += OnDayPhaseChanged;
            time = StaticStates.Get<TimeState>();
            inGameDays.Add(new FirstDay(matchingEntities));
            inGameDays.Add(new SecondDay(matchingEntities));
        }

        private void OnDayPhaseChanged(DayPhase dayPhase)
        {
            switch (dayPhase)
            {
                case DayPhase.Morning:
                    ResetNPCs();
                    //Fade handled by the new day flow. 
                    break;
                case DayPhase.Open:
                    Interface.Instance.BlackFader.FadeToBlack(4.0f, "Opening Time", () =>
                    {
                        ResetNPCs();
                        EventSystem.StartDrinkMakingEvent.Invoke();
                    });
                    break;
                case DayPhase.Night:
                    ResetNPCs();
                    Interface.Instance.BlackFader.FadeToBlack(4.0f, "After Hours");
                    EventSystem.EndDrinkMakingEvent.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("dayPhase", dayPhase, null);
            }
        }

        private void ResetNPCs()
        {
            initPeople.ForEach(person => ActionManagerSystem.Instance.TryClearActionsForEntity(person));
            initPeople.ForEach(person => person.GetState<PersonAnimationState>().ResetAnimationState());
            Locations.ResetPeopleToSpawnPoints(initPeople);
        }

        public void Tick(List<Entity> matchingEntities)
        {
            var currentTime = time.Time;
            if (currentTime == Constants.GameStartTime && !GameSettings.SkipFirstDayFadein && !DoneFirstDayFadeIn)
            {
                StaticStates.Get<TimeState>().TriggerDayTransition.Invoke("Day 1", false, true);
                DoneFirstDayFadeIn = true;
            }

            if (time.GameEnded || dayPhase.CurrentDayPhase != DayPhase.Open)
            {
                return;
            }

            if (currentTime != lastTime)
            {
                currentDay = UpdateDay(currentTime, matchingEntities);

                var timeDifferenceInMin = (currentTime - lastTime).Minutes;
                for (var elapsedMin = 1; elapsedMin <= timeDifferenceInMin; elapsedMin++)
                {
                    lastTime = lastTime.AddMinutes(1);
                    if (!GameSettings.DisableStory)
                    {
                        currentDay.UpdateDay(lastTime, matchingEntities);
                    }
                }
                lastTime = currentTime;
            }
        }

        private Day UpdateDay(DateTime currentTime, List<Entity> matchingEntities)
        {
            if (currentTime.Hour >= Constants.DayEndHour)
            {
                TriggerEndOfDayAfterDelay(currentTime, matchingEntities);
                currentTime = currentTime.AddHours(Constants.NightLengthInHours);
                time.Time = currentTime;
                lastTime = currentTime;

                if (currentTime.Day - 1 < inGameDays.Count)
                {
                    StaticStates.Get<TimeState>().TriggerDayTransition.Invoke(string.Format("Day {0}", currentTime.Day), true, true);
                }

                if (currentTime.Day - 1 >= inGameDays.Count)
                {
                    StaticStates.Get<TimeState>().TriggerEndOfGame.Invoke();
                    StaticStates.Get<TimeState>().GameEnded = true;
                    UnityEngine.Debug.Log("Reached the end of the last day!");
                    return inGameDays[0];
                }
            }

            return inGameDays[currentTime.Day - 1];
        }

        private void TriggerEndOfDayAfterDelay(DateTime currentTime, List<Entity> matchingEntities)
        {
            var dayToEnd = inGameDays[currentTime.Day - 1];
            DOTween.Sequence().SetDelay(3.0f).OnComplete(() => dayToEnd.OnEndOfDay(matchingEntities)); //HAX
        }
    }
}
