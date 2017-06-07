using System;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using UnityEngine;
using Assets.Framework.States;
using Assets.Scripts.Util;

namespace Assets.Scripts.Systems
{
    class TimeSystem : IFrameSystem, IInitSystem
    {
        private float secondsSinceLastMinute;

        private TimeState timeState;
        private DayPhaseState dayPhase;

        public void OnInit()
        {
            dayPhase = StaticStates.Get<DayPhaseState>();
            timeState = StaticStates.Get<TimeState>();
        }

        public void OnFrame()
        {
            if (dayPhase.CurrentDayPhase != DayPhase.Open)
            {
                return;
            }

            var dt = Time.deltaTime;
            secondsSinceLastMinute += dt;
            if (secondsSinceLastMinute >= Constants.SecondsPerGameMinute)
            {

                timeState.gameTime.IncrementMinute();
                secondsSinceLastMinute = Math.Max(secondsSinceLastMinute - Constants.SecondsPerGameMinute, 0f);
            }
        }
    }
}
