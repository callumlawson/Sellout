using System;
using System.Collections.Generic;

namespace Assets.Framework.States
{
    //TODO: This isn't a great patern should be repaced with an entity by convention (e.g. -1).
    public static class StaticStates
    {
        private static readonly List<IState> States = new List<IState>();

        public static void Add(IState state)
        {
            States.Add(state);
        }
   
        public static T Get<T>() where T : IState
        {
            foreach (var state in States)
            {
                if (state.GetType() == typeof(T))
                {
                    return (T)state;
                }
            }
            throw new InvalidOperationException(string.Format("Static state with type {0} requested but not found!", typeof(T)));
        }
    }
}
