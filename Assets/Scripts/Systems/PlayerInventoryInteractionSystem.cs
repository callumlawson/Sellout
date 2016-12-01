using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Framework.States;
using Assets.Scripts.Util.Events;

namespace Assets.Scripts.Systems
{
    class PlayerInventoryInteractionSystem : IInitSystem
    {
        private Entity player;

        public void OnInit()
        {
            EventSystem.onClickInteraction += OnClickInteraction;
            player = StaticStates.Get<PlayerState>().Player;
        }

        private void OnClickInteraction(ClickEvent clickEvent)
        {
            if (clickEvent.target.HasState<InventoryState>())
            {
                TryToGiveItem(clickEvent.target);
            }
        }
        
        private void TryToGiveItem(Entity to)
        {
            var playerInventory = player.GetState<InventoryState>();
            var childToMove = playerInventory.child;
            if (childToMove != null)
            {
                EventSystem.BroadcastEvent(new InventoryRequestEvent(player, to, childToMove));
            }
        }
    }
}
