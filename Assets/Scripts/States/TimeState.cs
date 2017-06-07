using Assets.Framework.States;
using System;

namespace Assets.Scripts.States
{
    [Serializable]
    public class TimeState : IState
    {
        public GameTime gameTime;

        public Action<string, bool, bool> TriggerDayTransition;
        public Action TriggerEndOfGame;
        public bool GameEnded;

        public TimeState(GameTime startTime)
        {
            gameTime = new GameTime(startTime.GetDay(), startTime.GetHour(), startTime.GetMinute());
        }
    }
}
