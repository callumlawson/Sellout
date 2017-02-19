using Assets.Framework.Entities;

namespace Assets.Scripts.Util.Events
{
    public class ClickEvent
    {
        public readonly Entity Target;
        public SerializableVector3 ClickPosition;
        public readonly int MouseButton;

        public ClickEvent(Entity target, SerializableVector3 clickPosition, int mouseButton)
        {
            Target = target;
            ClickPosition = clickPosition;
            MouseButton = mouseButton;
        }
    }
}
