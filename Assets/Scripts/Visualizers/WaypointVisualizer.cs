using Assets.Framework.Entities;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visualizers
{
    [UsedImplicitly]
    public class WaypointVisualizer : MonoBehaviour, IEntityVisualizer
    {
        public void OnStartRendering(Entity entity)
        {
            //Do Nothing.
        }

        public void OnFrame()
        {
            transform.GetChild(0).gameObject.SetActive(GameSettings.IsDebugOn);
        }

        public void OnStopRendering(Entity entity)
        {
            //Do Nothing.
        }
    }
}