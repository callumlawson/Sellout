using System.Collections.Generic;
using Assets.Scripts.GameActions.Framework;

namespace Assets.Scripts.GameActions.Composite
{
    public abstract class CompositeAction : GameAction
    {
        protected readonly List<GameAction> Actions = new List<GameAction>();
        public void Add(GameAction action) { Actions.Add(action); }
    }
}
