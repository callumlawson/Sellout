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
        private float secondsSinceLastMinute;

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

                timeState.time = timeState.time.AddMinutes(1.0f);
                secondsSinceLastMinute = Math.Max(secondsSinceLastMinute - secondsPerGameMinute, 0f);
            }
        }
    }
}
