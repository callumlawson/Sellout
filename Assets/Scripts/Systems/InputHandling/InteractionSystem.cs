using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.States;

namespace Assets.Scripts.Systems.Input
{
    internal class InteractionSystem : IFrameEntitySystem
    {
        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(InteractiveState) };
        }

        public void OnFrame(List<Entity> matchingEntities)
        {
            //Do nothing.
        }
    }
}