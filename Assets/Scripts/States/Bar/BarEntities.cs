using System;
using Assets.Framework.States;
using Assets.Framework.Entities;

namespace Assets.Scripts.States.Bar
{
    [Serializable]
    public class BarEntities : IState
    {
        public Entity ReceiveSpot;
    }
}
