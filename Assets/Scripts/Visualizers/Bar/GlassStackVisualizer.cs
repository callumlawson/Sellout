using System;
using Assets.Framework.Entities;
using Assets.Scripts.States;
using UnityEngine;
using Assets.Scripts.Systems;
using JetBrains.Annotations;
using DG.Tweening;

namespace Assets.Scripts.Visualizers.Bar
{
    class GlassStackVisualizer : MonoBehaviour, IEntityVisualizer
    {
        [UsedImplicitly] public Transform slot;

        private Entity entity;
        private InventoryState inventory;

        public void Awake()
        {
            slot = GetComponentInChildren<SlotVisualizer>().transform;
        }

        public void OnStartRendering(Entity entity)
        {
            this.entity = entity;
            inventory = entity.GetState<InventoryState>();
            EventSystem.TakeGlass += OnTakeGlass;
        }

        private void OnTakeGlass(TakeGlassRequest request)
        {
            if (request.stack == entity)
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
