using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Util;

namespace Assets.Scripts.Systems
{
    class DayDirectorSystem : ITickEntitySystem, IInitSystem
    {
        private TimeState time;
        private Day currentDay;
        private DateTime lastTime;

        private List<Day> InGameDays = new List<Day> { new FirstDay(), new SecondDay() };

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
            var currentTime = time.time;
            if (currentTime != lastTime && (currentTime.Day < InGameDays.Count));
            {
                currentDay = InGameDays[currentTime.Day - 1];
                var timeDifferenceInMin = (currentTime - lastTime).Minutes;
                for (int elapsedMin = 1; elapsedMin <= timeDifferenceInMin; elapsedMin++)
                {
                    lastTime = lastTime.AddMinutes(1);
                    currentDay.UpdateDay(lastTime, matchingEntities);
                }
                lastTime = currentTime;
            }
        }
    }
}
