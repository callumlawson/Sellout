using Assets.Scripts.Util.Events;

namespace Assets.Scripts.Systems
{
    static class EventSystem
    {
        public delegate void OnClickEvent(ClickEvent clickEvent);
        public static OnClickEvent onClickInteraction = null;

        public delegate void OnOpenDrinkMenuEvent();
        public static OnOpenDrinkMenuEvent onOpenDrinkMenuEvent = null;

        public delegate void OnInventoryRequestEvent(InventoryRequestEvent inventoryRequestEvent);
        public static OnInventoryRequestEvent onInventoryRequestEvent = null;

        public delegate void OnInventoryEvent(InventoryEvent inventoryEvent);
        public static OnInventoryEvent onInventoryEvent = null;

        public static void BroadcastEvent(ClickEvent clickEvent)
        {
            onClickInteraction(clickEvent);
        }

        public static void BroadcastEvent(InventoryRequestEvent inventoryRequestEvent)
        {
            onInventoryRequestEvent(inventoryRequestEvent);
        }

        public static void BroadcastEvent(InventoryEvent inventoryEvent)
        {
            onInventoryEvent(inventoryEvent);
        }

        public static void BroadcastEvent(OpenDrinkMakingMenuEvent openDrinkMakingMenuEvent)
        {
            onOpenDrinkMenuEvent();
        }
    }
}
