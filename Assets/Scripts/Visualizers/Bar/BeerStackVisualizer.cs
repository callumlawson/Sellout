using System.Collections.Generic;
using Assets.Framework.States;
using Assets.Scripts.Util;
using Assets.Scripts.States;

namespace Assets.Scripts.Visualizers.Bar
{
    public class BeerStackVisualizer : ItemStackVisualizer
    {
        public override List<IState> GetNewStackItem()
        {
            return new List<IState>
            {
                new PrefabState(Prefabs.Beer),
                new DrinkState(new DrinkState(DrinkRecipes.Beer.Contents)),
                new PositionState(transform.position),
                new InventoryState(),
                new InteractiveState()
            };
        }
    }
}
