using Assets.Scripts.Util;

namespace Assets.Scripts.Systems
{
    static class EventSystem
    {
        public delegate void OnEventBroadcast(Message message);
        public static OnEventBroadcast onEventBroadcast;

        public static void BroadcastMessage(Message message)
        {
            onEventBroadcast(message);
        }
    }
}
