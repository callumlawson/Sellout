using System;
using Assets.Framework.Entities;
using Assets.Scripts.Util.Events;

namespace Assets.Scripts.Systems
{
    public struct ParentingRequest
    {
        public Entity EntityFrom;
        public Entity EntityTo;
        public Entity Mover;
    }

    public struct TakeGlassRequest
    {
        public Entity Requester;
        public Entity stack;       
    }

    static class EventSystem
    {
        public static Action<ParentingRequest> ParentingRequestEvent = delegate {  };
        public static Action StartDrinkMakingEvent = delegate {  };
        public static Action EndDrinkMakingEvent = delegate {  };

        public delegate void OnClickEvent(ClickEvent clickEvent);
        public static OnClickEvent onClickInteraction = null;

        public static Action<TakeGlassRequest> TakeGlass = delegate { };

        public static void BroadcastEvent(ClickEvent clickEvent)
        {
            onClickInteraction(clickEvent);
        }
    }
}
