﻿using System.Collections.Generic;
using Assets.Framework.States;
using Assets.Scripts.Util;
using Assets.Scripts.States;

namespace Assets.Scripts.Visualizers.Bar
{
    public class GlassStackVisualizer : ItemStackVisualizer
    {
        public override List<IState> GetNewStackItem()
        {
            return new List<IState>
            {
                new PrefabState(Prefabs.Drink),
                new DrinkState(new DrinkState()),
                new PositionState(transform.position),
                new InventoryState()
            };
        }
    }
}
