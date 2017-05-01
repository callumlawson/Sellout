using Assets.Framework.States;
using Assets.Scripts.States;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    class ClockUIVisualizer : MonoBehaviour
    {
        [UsedImplicitly] public Text Day;
        [UsedImplicitly] public Text Time;

        private const string DayText = "Day {0}";
        private const string TimeText = "{0:00}:{1:00}";

        private TimeState timeState;
        private DayPhaseState dayPhase;
        
        [UsedImplicitly]
        public void Update()
        {
            if (timeState == null)
            {
                timeState = StaticStates.Get<TimeState>();
            }

            if (dayPhase == null)
            {
                dayPhase = StaticStates.Get<DayPhaseState>();
            }

            if (timeState == null || dayPhase == null)
            {
                return;
            }

            if (dayPhase.CurrentDayPhase == DayPhase.Open)
            {
                Day.text = string.Format(DayText, timeState.Time.Day);
                Time.text = string.Format(TimeText, timeState.Time.Hour, timeState.Time.Minute);
            }
            else
            {
                Time.text = dayPhase.CurrentDayPhase.ToString();       
            }
        }
    }
}
