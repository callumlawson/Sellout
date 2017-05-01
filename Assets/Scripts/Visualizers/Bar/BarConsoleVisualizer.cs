using Assets.Framework.States;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.Visualizers.Bar
{
    public class BarConsoleVisualizer : MonoBehaviour
    {
        public GameObject ScreenOn;
        public GameObject ScreenOff;

        void Start()
        {
            var dayPhaseState = StaticStates.Get<DayPhaseState>();
            dayPhaseState.DayPhaseChangedTo += DayPhaseChangedTo;
        }

        private void DayPhaseChangedTo(DayPhase newPhase)
        {
            if (newPhase == DayPhase.Open)
            {
                ScreenOn.SetActive(true);
                ScreenOff.SetActive(false);
            }
            else
            {
                ScreenOn.SetActive(false);
                ScreenOff.SetActive(true);
            }
        }
    }
}
