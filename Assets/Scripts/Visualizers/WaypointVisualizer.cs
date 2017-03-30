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
        private Collider ourCollider;

        public void OnStartRendering(Entity entity)
        {
            ourRenderer = GetComponentInChildren<MeshRenderer>();
            ourCollider = GetComponentInChildren<Collider>();
        }

        public void OnFrame()
        {
            ourRenderer.enabled = GameSettings.IsDebugOn;
            ourCollider.enabled = GameSettings.IsDebugOn;
        }

        public void OnStopRendering(Entity entity)
        {
            //Do Nothing.
        }
    }
}