using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visualizers
{
    class SlotVisualizer : MonoBehaviour
    {
        [UsedImplicitly] public bool canHoldDrinks;

        public bool CanHoldDrinks()
        {
            return canHoldDrinks;
        }
    }
}
