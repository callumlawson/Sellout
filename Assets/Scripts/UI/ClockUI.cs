using Assets.Framework.States;
using Assets.Scripts.States;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    class ClockUI : MonoBehaviour
    {
#pragma warning disable 649
        public Text Day;
        public Text Time;
#pragma warning restore 649

        private readonly string DayText = "Day {0}";
        private readonly string TimeText = "{0:00}:{1:00}";

        private TimeState timeState;
        
        void Update()
        {
            if (timeState == null)
            {
                timeState = StaticStates.Get<TimeState>();
            }

            if (timeState == null)
            {
                return;
            }

            Day.text = string.Format(DayText, timeState.day);
            Time.text = string.Format(TimeText, timeState.hour, timeState.minute);
        }
    }
}
