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

        public void OnInit()
        {
            timeState = StaticStates.Get<TimeState>();
        }

        public void OnFrame()
        {
            var dt = Time.deltaTime;
            secondsSinceLastMinute += dt;
            if (secondsSinceLastMinute >= Constants.SecondsPerGameMinute)
            {

                timeState.time = timeState.time.AddMinutes(1.0f);
                secondsSinceLastMinute = Math.Max(secondsSinceLastMinute - Constants.SecondsPerGameMinute, 0f);
            }
        }
    }
}
