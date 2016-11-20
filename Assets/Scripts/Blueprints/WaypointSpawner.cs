﻿using System.Collections.Generic;
using Assets.Framework.States;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Blueprints
{
    public class WaypointSpawner : MonoBehaviour, IEntityBlueprint
    {
        [UsedImplicitly] public GameObject NextWaypoint;

        public List<IState> EntityToSpawn()
        {
            return new List<IState>
            {
                new PositionState(transform.position),
                new BlueprintGameObjectState(gameObject),
                new WaypointState(),
                new PrefabState(Prefabs.Waypoint)
            };
        }
    }
}