using Assets.Framework.Entities;
using Assets.Framework.States;
using System;

namespace Assets.Scripts.States
{
    [Serializable]
    class InventoryState : IState
    {
        public Entity child { get; private set; }

        public InventoryState()
        {
            child = null;
        }

        public void SetChild(Entity newChild)
        {
            child = newChild;
        }

        public void RemoveChild()
        {
            child = null;
        }

        public override string ToString()
        {
            return "Inventory: " + (child != null ? child.EntityId.ToString() : "<empty>");
        }
    }
}
