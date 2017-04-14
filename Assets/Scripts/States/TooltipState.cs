using Assets.Framework.States;

namespace Assets.Scripts.States
{
    class TooltipState : IState
    {
        public string Tooltip;

        public TooltipState(string tooltip)
        {
            Tooltip = tooltip;
        }

        public override string ToString()
        {
            return Tooltip;
        }
    }
}
