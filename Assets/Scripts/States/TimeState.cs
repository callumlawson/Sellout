using Assets.Framework.States;
using System;

namespace Assets.Scripts.States
{
    [Serializable]
    class TimeState : IState
    {
        public DateTime time;

        public TimeState(DateTime time)
        {
            this.time = time;
        }
    }
}
