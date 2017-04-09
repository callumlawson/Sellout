using System;
using Assets.Framework.Entities;
using Assets.Scripts.Util.Events;
using Assets.Scripts.Util;

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

    public struct DrinkOrder
    {
        public DrinkRecipe Recipe;
        public string OrdererName;
        public string OrdererSpecies;
    }

    static class EventSystem
    {
        public static Action PauseEvent = delegate { };
        public static Action ResumeEvent = delegate { };

        public static Action<ParentingRequest> ParentingRequestEvent = delegate {  };
        public static Action StartDrinkMakingEvent = delegate {  };
        public static Action EndDrinkMakingEvent = delegate {  };
        public static Action<DrinkOrder> StartDrinkOrderEvent = delegate {  };
        public static Action EndDrinkOrderEvent = delegate {  };

        public delegate void OnClickEvent(ClickEvent clickEvent);
        public static OnClickEvent onClickInteraction = null;

        public static Action<TakeGlassRequest> TakeGlass = delegate { };

        public static void BroadcastEvent(ClickEvent clickEvent)
        {
            onClickInteraction(clickEvent);
        }
    }
}
