using System;
using Assets.Framework.States;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.States
{
    [Serializable]
    class ColorState : IState
    {
        public SerializableColor Color;

        public ColorState(Color color)
        {
            Color = color;
        }

        public override string ToString()
        {
            return string.Format("Color: {0}", Color);
        }
    }
}
