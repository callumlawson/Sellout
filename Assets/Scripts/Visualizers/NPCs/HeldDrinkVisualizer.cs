using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visualizers.NPCs
{
    public class HeldDrinkVisualizer : MonoBehaviour
    {
        [UsedImplicitly]
        void Update()
        {
            transform.rotation = Quaternion.identity;
        }
    }
}
