using Assets.Framework.Systems;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    class DebugControlsSystem : IFrameSystem
    {
        public void OnFrame()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                EventSystem.DayPhaseIncrementRequest.Invoke();
            }
        }
    }
}
