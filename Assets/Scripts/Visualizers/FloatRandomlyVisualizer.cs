using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visualizers
{
    class FloatRandomlyVisualizer : MonoBehaviour
    {
        private float initYPosition;

        [UsedImplicitly]
        void Start()
        {
            initYPosition = transform.position.y;
            gameObject.transform.DOMoveY(initYPosition + 0.15f, 3.5f).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
