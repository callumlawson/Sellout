using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Visualizers
{
    [UsedImplicitly]
    class NameStateVisualizer : MonoBehaviour, IEntityVisualizer
    {
        private readonly Interface interfaceMonobehaviour = Interface.Instance;

        private PositionState positionState;
        private GameObject nameTag;
        private RectTransform nameTagRectTransform;
        private Vector3 offset;

        public void OnStartRendering(Entity entity)
        {
            positionState = entity.GetState<PositionState>();
            nameTag = Instantiate(Resources.Load(Prefabs.NameTagUI) as GameObject);
            nameTag.transform.SetParent(interfaceMonobehaviour.gameObject.transform);
            nameTag.GetComponent<Text>().text = entity.GetState<NameState>().Name;
            nameTagRectTransform = nameTag.GetComponent<RectTransform>();
            offset = new Vector3(0, entity.GetState<NameState>().VerticalOffset, 0);
        }

        public void OnFrame()
        {
            nameTagRectTransform.anchoredPosition = interfaceMonobehaviour.GetComponent<Canvas>().WorldToCanvas(positionState.Position + offset);
        }

        public void OnStopRendering(Entity entity)
        {
            Destroy(nameTag);
        }
    }
}
