using System;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Framework.Util;
using UnityEngine;

namespace Assets.Scripts.States
{
    [Serializable]
    class WaypointState : IState
    {
        public Entity NextWaypoint;

        [NonSerialized] private GameObject nextWaypointGameObject;

        public WaypointState(GameObject nextWaypoint)
        {
            nextWaypointGameObject = nextWaypoint;
        }

        public void ResolveWaypointGameobjectToEntity(EntityStateSystem ess)
        {
            NextWaypoint = ess.GetEntity(nextWaypointGameObject.GetEntityId());
        }

        public override string ToString()
        {
            return NextWaypoint != null ? string.Format("Next Waypoint: {0}", NextWaypoint) : "No next waypoint";
        }
    }
}
