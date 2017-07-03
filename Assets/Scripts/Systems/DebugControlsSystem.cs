using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    class DebugControlsSystem : IFrameSystem
    {
        public void OnFrame()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                StaticStates.Get<DayPhaseState>().IncrementDayPhase();
            }
        }
    }
}
