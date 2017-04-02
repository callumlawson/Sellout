using Assets.Framework.States;
using System;

namespace Assets.Scripts.States
{
    [Serializable]
    public class TimeState : IState
    {
        public DateTime time;
        public readonly DateTime startTime;
        public Action<int, bool> TriggerDayTransition;

        public TimeState(DateTime time)
        {
            startTime = time;
            this.time = time;
        }
    }
}
