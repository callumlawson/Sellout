using Assets.Framework.Entities;

namespace Assets.Scripts.Util.Events
{
    public class ClickEvent
    {
        public readonly Entity target;

        public ClickEvent(Entity target)
        {
            this.target = target;
        }
    }
}
