using System;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions;
using Assets.Scripts.Util.Events;

namespace Assets.Scripts.Systems
{
    public struct ParentingRequest
    {
        public Entity EntityFrom;
        public Entity EntityTo;
        public Entity Mover;
    }

    public struct TakeStackItemRequest
    {
        public Entity Requester;
        public Entity Stack;       
    }

    static class EventSystem
    {
        public static Action PauseEvent = delegate { };
        public static Action ResumeEvent = delegate { };

        public static Action<ParentingRequest> ParentingRequestEvent = delegate {  };
        public static Action StartDrinkMakingEvent = delegate {  };
        public static Action EndDrinkMakingEvent = delegate {  };
        public static Action<DrinkOrders.DrinkOrder> StartDrinkOrderEvent = delegate {  };
        public static Action EndDrinkOrderEvent = delegate {  };

        public delegate void OnClickEvent(ClickEvent clickEvent);
        public static OnClickEvent onClickInteraction = null;

        public static Action<TakeStackItemRequest> TakeStackItem = delegate { };

        public static void BroadcastEvent(ClickEvent clickEvent)
        {
            onClickInteraction(clickEvent);
        }
    }
}
