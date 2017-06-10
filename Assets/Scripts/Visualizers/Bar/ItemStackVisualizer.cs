using Assets.Framework.Entities;
using UnityEngine;
using Assets.Scripts.Systems;
using JetBrains.Annotations;
using DG.Tweening;
using Assets.Framework.States;
using System.Collections.Generic;

namespace Assets.Scripts.Visualizers.Bar
{
    abstract public class ItemStackVisualizer : MonoBehaviour, IEntityVisualizer
    {
        [UsedImplicitly] private Transform slot;

        private Entity entity;
    
        public void Awake()
        {
            slot = GetComponentInChildren<SlotVisualizer>().transform;
        }

        abstract public List<IState> GetNewStackItem();

        public void OnStartRendering(Entity entity)
        {
            this.entity = entity;
            EventSystem.TakeStackItem += OnTakeStackItem;
        }

        private void OnTakeStackItem(TakeStackItemRequest request)
        {
            if (request.Stack == entity)
            {
                var originalY = slot.position.y;
                slot.localPosition = new Vector3(slot.localPosition.x, -0.6f, slot.localPosition.z);
                slot.DOMoveY(originalY, 1.0f);
            }
        }

        public void OnStopRendering(Entity entity)
        {
            // do nothing
        }

        public void OnFrame()
        {
            // do nothing
        }
    }
}
