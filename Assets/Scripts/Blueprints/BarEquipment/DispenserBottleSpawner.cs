﻿using System.Collections.Generic;
using Assets.Framework.States;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Blueprints.BarEquipment
{
    public class DispenserBottleSpawner : MonoBehaviour, IEntityBlueprint
    {
        public List<IState> EntityToSpawn()
        {
            return new List<IState>
            {
                new PositionState(transform.position),
                new RotationState(transform.rotation),
                new PrefabState(Prefabs.DispensingBottle),
                new TooltipState("Dispenses alcohol."),
                new InventoryState()
            };
        }
    }
}
