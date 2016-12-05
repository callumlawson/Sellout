using System;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using UnityEngine;
using Assets.Framework.States;

namespace Assets.Scripts.Systems
{
    class TimeSystem : IFrameSystem, IInitSystem
    {
        private float secondsPerGameMinute = 1f;
        private float secondsSinceLastMinute = 0f;

        private TimeState timeState;

        public void OnInit()
        {
            timeState = StaticStates.Get<TimeState>();
        }

        public void OnFrame()
        {
            var dt = Time.deltaTime;
            secondsSinceLastMinute += dt;
            if (secondsSinceLastMinute >= secondsPerGameMinute)
            {
                var newDay = timeState.day;
                var newHour = timeState.hour;
                var newMinute = timeState.minute + 1;
                
                if (newMinute >= 60)
                {
                    newMinute = 0;
                    newHour++;
                }

                if (newHour >= 24)
                {
                    newHour = 0;
                    newDay++;
                }

                timeState.day = newDay;
                timeState.hour = newHour;
                timeState.minute = newMinute;

                secondsSinceLastMinute = Math.Max(secondsSinceLastMinute - secondsPerGameMinute, 0f);
            }
        }
    }
}
