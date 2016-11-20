using System;
using Assets.Framework.Entities;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    [Serializable]
    class WaypointState : IState
    {
        public Entity NextWaypoint;

        public override string ToString()
        {
            return NextWaypoint != null ? string.Format("Next Waypoint Id: {0}", NextWaypoint.EntityId) : "No next waypoint";
        }
    }
}
