using Assets.Framework.Systems;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    class HierarchyManipulationSystem : IInitSystem
    {
        public void OnInit()
        {
            EventSystem.ParentingRequestEvent += OnParentingRequestEvent;
        }

        private void OnParentingRequestEvent(ParentingRequest parentingRequest)
        {
            var entityTo = parentingRequest.EntityTo;
            var mover = parentingRequest.Mover;

            if (entityTo != null && !entityTo.HasState<InventoryState>())
            {
                Debug.LogError("Target exists but does not have an inventory.");
            }

            if (mover != null && (entityTo == null || (entityTo.GetState<InventoryState>().Child == null && entityTo.GetState<InventoryState>().AcceptingChildren)))
            {
                var moverHierarchy = mover.GetState<InventoryState>();

                if (mover.GetState<InventoryState>().Parent != null)
                {
                    var fromInventoryState = mover.GetState<InventoryState>().Parent.GetState<InventoryState>();
                    fromInventoryState.RemoveChild();
                }

                if (entityTo != null)
                {
                    entityTo.GetState<InventoryState>().SetChild(mover);
                }

                moverHierarchy.SetParent(entityTo);

                EventSystem.ParentingRequestSucceeded.Invoke(parentingRequest);
            }
        }
    }
}
