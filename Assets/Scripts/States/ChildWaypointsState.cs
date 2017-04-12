using Assets.Framework.Entities;
using Assets.Framework.States;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.States
{
    [Serializable]
    class ChildWaypointsState : IState
    {
        public List<Entity> Children { get; private set; }

        public Action<ChildWaypointsState> ChildWaypointsUpdated = delegate { };

        public ChildWaypointsState()
        {
            Children = new List<Entity>();
        }

        public void AddChild(Entity newChild)
        {
            Children.Add(newChild);
            ChildWaypointsUpdated.Invoke(this);
        }

        public void RemoveChild(Entity child)
        {
            Children.Remove(child);
            ChildWaypointsUpdated.Invoke(this);
        }

        public override string ToString()
        {
            return "Children: { " + string.Join(",", Children.Select(entity => entity.EntityId.ToString()).ToArray()) + " }";
        }
    }
}
