using Assets.Framework.Entities;
using Assets.Framework.States;
using System;

namespace Assets.Scripts.States
{
    [Serializable]
    class HierarchyState : IState
    {
        public Entity Parent { get; private set; }
        public Entity Child { get; private set; }

        public Action<HierarchyState> HierarchyUpdated = delegate {  };

        public HierarchyState()
        {
            Child = null;
            Parent = null;
        }

        public void SetChild(Entity newChild)
        {
            Child = newChild;
            HierarchyUpdated.Invoke(this);
        }

        public void RemoveChild()
        {
            Child = null;
            HierarchyUpdated.Invoke(this);
        }

        public void SetParent(Entity newParent)
        {
            Parent = newParent;
            HierarchyUpdated.Invoke(this);
        }

        public void RemoveParent()
        {
            Parent = null;
            HierarchyUpdated.Invoke(this);
        }

        public override string ToString()
        {
            return "Child: " + (Child != null ? Child.EntityId.ToString() : "<empty>") +
                   " Parent: " + (Parent != null ? Parent.EntityId.ToString() : "<empty>");
        }
    }
}
