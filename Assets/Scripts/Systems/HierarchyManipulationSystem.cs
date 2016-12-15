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
            var entityToHierarchy = entityTo.GetState<HierarchyState>();
            
            if (mover != null && entityToHierarchy.Child == null)
            {
                var moverHierarchy = mover.GetState<HierarchyState>();

                if (mover.GetState<HierarchyState>().Parent != null)
                {
                    var fromInventoryState = mover.GetState<HierarchyState>().Parent.GetState<HierarchyState>();
                    fromInventoryState.RemoveChild();
                }
                
                entityToHierarchy.SetChild(mover);
                moverHierarchy.SetParent(entityTo);
            }
        }
    }
}
