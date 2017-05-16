using Assets.Framework.Entities;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visualizers
{
    [UsedImplicitly]
    class MixologyBookVisualizer : MonoBehaviour, IEntityVisualizer
    {
        public GameObject MixologyUI;

        private ActiveState activeState;

        public void OnStartRendering(Entity entity)
        {
            activeState = entity.GetState<ActiveState>();
        }

        public void OnFrame()
        {

        }

        public void Update()
        {
            if (MixologyUI.activeSelf != activeState.IsActive)
            {
                MixologyUI.SetActive(activeState.IsActive);

                if (activeState.IsActive)
                {
                    EventSystem.PauseEvent.Invoke();
                }
                else
                {
                    EventSystem.ResumeEvent.Invoke();
                }
            }
        }

        public void OnStopRendering(Entity entity)
        {
            //Do nothing
        }

        public void Close()
        {
            activeState.IsActive = false;
        }
    }
}
