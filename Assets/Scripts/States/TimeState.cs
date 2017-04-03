using Assets.Framework.States;
using System;

namespace Assets.Scripts.States
{
    [Serializable]
    public class TimeState : IState
    {
        public DateTime Time;
        public Action<string, bool, bool> TriggerDayTransition;
        public Action TriggerEndOfGame;
        public bool GameEnded;

        public TimeState(DateTime time)
        {
            Time = time;
        }
    }
}
