using Assets.Framework.Systems;
using Assets.Scripts.Util.Events;
using Assets.Scripts.States;
using UnityEngine;
using Assets.Framework.Entities;
using Assets.Scripts.Visualizers.Slots;

namespace Assets.Scripts.Systems
{
    class VisibleSlotSystem : IInitSystem
    {
        public void OnInit()
        {
            EventSystem.onInventoryEvent += OnInventoryEvent;
        }

        private void OnInventoryEvent(InventoryEvent inventoryEvent)
        {
            var childGameObject = inventoryEvent.child.GameObject;
            if (childGameObject != null)
            {
                if (inventoryEvent.to.HasState<VisibleSlotState>())
                {
                    MoveChildIntoVisibleSlot(inventoryEvent.to, childGameObject);
                }
                else
                {
                    MoveChildOutOfView(childGameObject);
                }
            }
        }

        private void MoveChildIntoVisibleSlot(Entity to, GameObject child)
        {
            var visibleSlot = to.GameObject.GetComponentInChildren<SlotVisualizer>();
            if (visibleSlot != null)
            {
                child.transform.parent = visibleSlot.transform;
                child.transform.localPosition = Vector3.zero;
                child.transform.localRotation = Quaternion.identity;
            }
            else
            {
                Debug.LogWarning("Receiving entity has a visible slot, but was unable to find it on the GameObject.");
                MoveChildOutOfView(child);
            }
        }

        private void MoveChildOutOfView(GameObject child)
        {
            child.transform.parent = null;
            child.transform.position = new Vector3(0, -10, 0);
        }
    }
}
