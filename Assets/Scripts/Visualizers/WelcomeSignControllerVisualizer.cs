using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visualizers
{
    public class WelcomeSignControllerVisualizer : MonoBehaviour {

        public static WelcomeSignControllerVisualizer Instance;

        [UsedImplicitly]
        public void Start()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        public void SetWelcomeSignActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
