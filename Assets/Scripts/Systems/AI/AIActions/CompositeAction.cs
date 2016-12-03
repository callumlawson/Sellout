using System.Collections.Generic;

namespace Assets.Scripts.Systems.AI.AIActions
{
    public abstract class CompositeAction : GameAction
    {
        protected readonly List<GameAction> Actions = new List<GameAction>();
        public void Add(GameAction action) { Actions.Add(action); }
    }
}
