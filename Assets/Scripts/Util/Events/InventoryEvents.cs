using Assets.Framework.Entities;

namespace Assets.Scripts.Util.Events
{
    public class InventoryRequestEvent
    {
        public readonly Entity from;
        public readonly Entity to;
        public readonly Entity child;

        public InventoryRequestEvent(Entity from, Entity to, Entity child)
        {
            this.from = from;
            this.to = to;
            this.child = child;
        }
    }

    public class InventoryEvent
    {
        public readonly Entity from;
        public readonly Entity to;
        public readonly Entity child;

        public InventoryEvent(Entity from, Entity to, Entity child)
        {
            this.from = from;
            this.to = to;
            this.child = child;
        }
    }
}
