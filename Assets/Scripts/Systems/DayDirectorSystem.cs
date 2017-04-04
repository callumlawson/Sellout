﻿using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using DG.Tweening;

namespace Assets.Scripts.Systems
{
    class DayDirectorSystem : ITickEntitySystem, IInitSystem
    {
        private TimeState time;
        private Day currentDay;
        private DateTime lastTime;

        private readonly List<Day> inGameDays = new List<Day> { new FirstDay(), new SecondDay() };

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(PersonState) };
        }
        public void OnInit()
        {
            time = StaticStates.Get<TimeState>();
        }

        public void Tick(List<Entity> matchingEntities)
        {
            if (time.GameEnded)
            {
                return;
            }

            var currentTime = time.Time;
            if (currentTime != lastTime)
            {
                if (currentTime == Constants.GameStartTime && !GameSettings.SkipFirstDayFadein)
                {
                    StaticStates.Get<TimeState>().TriggerDayTransition.Invoke("Day 1", false, true);
                }

                currentDay = UpdateDay(currentTime, matchingEntities);

                var timeDifferenceInMin = (currentTime - lastTime).Minutes;
                for (int elapsedMin = 1; elapsedMin <= timeDifferenceInMin; elapsedMin++)
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
            DOTween.Sequence().SetDelay(3.0f).OnComplete(() => dayToEnd.EndDay(matchingEntities)); //HAX
        }
    }
}
