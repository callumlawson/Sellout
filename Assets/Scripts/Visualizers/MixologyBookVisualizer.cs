using Assets.Framework.Entities;
using Assets.Scripts.States;
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
            if (MixologyUI.activeSelf != activeState.IsActive)
            {
                MixologyUI.SetActive(activeState.IsActive);
            }
        }

        public void OnStopRendering(Entity entity)
        {
            //Do nothing
        }
    }
}
