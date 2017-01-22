using Assets.Framework.States;
using Assets.Scripts.Util.Clothing;
using System;

namespace Assets.Scripts.States
{
    [Serializable]
    class ClothingState : IState
    {
        public ClothingTopType top;
        public ClothingBottomType bottom;

        public ClothingState(ClothingTopType top, ClothingBottomType bottom)
        {
            this.top = top;
            this.bottom = bottom;
        }
    }
}
