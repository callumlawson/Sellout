using Assets.Framework.Systems;
using Assets.Scripts.States;

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
            var entityToHierarchy = entityTo.GetState<InventoryState>();
            
            if (mover != null && entityToHierarchy.Child == null)
            {
                var moverHierarchy = mover.GetState<InventoryState>();

                if (mover.GetState<InventoryState>().Parent != null)
                {
                    var fromInventoryState = mover.GetState<InventoryState>().Parent.GetState<InventoryState>();
                    fromInventoryState.RemoveChild();
                }
                
                entityToHierarchy.SetChild(mover);
                moverHierarchy.SetParent(entityTo);
            }
        }
    }
}
