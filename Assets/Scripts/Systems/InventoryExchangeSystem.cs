using Assets.Framework.Systems;
using Assets.Scripts.Util.Events;
using Assets.Scripts.States;

namespace Assets.Scripts.Systems
{
    class InventoryExchangeSystem : IInitSystem
    {
        public void OnInit()
        {
            EventSystem.onInventoryRequestEvent += OnInventoryRequestEvent;
        }

        private void OnInventoryRequestEvent(InventoryRequestEvent inventoryRequestEvent)
        {
            var to = inventoryRequestEvent.to;
            var childToMove = inventoryRequestEvent.child;

            var toInventoryState = to.GetState<InventoryState>();
            
            if (childToMove != null && toInventoryState.child == null)
            {
                var from = inventoryRequestEvent.from;
                if (from != null && from.HasState<InventoryState>())
                {
                    var fromInventoryState = from.GetState<InventoryState>();
                    fromInventoryState.RemoveChild();
                }
                
                toInventoryState.SetChild(childToMove);
                EventSystem.BroadcastEvent(new InventoryEvent(from, to, childToMove));
            }
        }
    }
}
