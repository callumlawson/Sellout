using Assets.Framework.Entities;

namespace Assets.Scripts.Util.Events
{
    public class ClickEvent
    {
        public readonly Entity Target;
        public SerializableVector3 ClickPosition;

        public ClickEvent(Entity target, SerializableVector3 clickPosition)
        {
            Target = target;
            ClickPosition = clickPosition;
        }
    }
}
