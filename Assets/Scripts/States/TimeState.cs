using Assets.Framework.States;
using System;
using Assets.Scripts.Util;

namespace Assets.Scripts.States
{
    [Serializable]
    public class TimeState : IState
    {
        public readonly GameTime GameTime;

        public TimeState(GameTime startTime)
        {
            GameTime = new GameTime(startTime.GetDay(), startTime.GetHour(), startTime.GetMinute());
        }
    }
}
