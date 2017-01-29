using Assets.Framework.Entities;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visualizers
{
    [UsedImplicitly]
    public class WaypointVisualizer : MonoBehaviour, IEntityVisualizer
    {
        private MeshRenderer ourRenderer;

        public void OnStartRendering(Entity entity)
        {
            ourRenderer = GetComponentInChildren<MeshRenderer>();
        }

        public void OnFrame()
        {
            ourRenderer.enabled = GameSettings.IsDebugOn;
        }

        public void OnStopRendering(Entity entity)
        {
            //Do Nothing.
        }
    }
}