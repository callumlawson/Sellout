using Assets.Framework.States;
using System;

namespace Assets.Scripts.States
{
    [Serializable]
    class TimeState : IState
    {
        public int day;
        public int hour;
        public int minute;

        public TimeState(int day, int hour, int minute)
        {
            this.day = day;
            this.hour = hour;
            this.minute = minute;
        }
    }
}
