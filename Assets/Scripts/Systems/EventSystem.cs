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
        public static Action<ClickEvent> OnClickedEvent = delegate {  };
        public static Action<TakeStackItemRequest> TakeStackItem = delegate { };
    }
}
