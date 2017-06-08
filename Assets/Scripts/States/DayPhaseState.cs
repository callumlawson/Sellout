using System;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    public enum DayPhase
    {
        Morning, Open, Night
    }

    [Serializable]
    class DayPhaseState : IState
    {
        public Action<DayPhase> DayPhaseChangedTo;
        public DayPhase CurrentDayPhase { get; private set; }

        private DayPhase DayPhase;

        public DayPhaseState(DayPhase dayPhase)
        {
            DayPhase = dayPhase;
        }

        public void SetDayPhase(DayPhase newDayPhase)
        {
            CurrentDayPhase = newDayPhase;
            if (DayPhaseChangedTo != null)
            {
                DayPhaseChangedTo.Invoke(newDayPhase);
            }
        }

        public void IncrementDayPhase()
        {
            switch (DayPhase)
            {
                case DayPhase.Morning:
                    DayPhase = DayPhase.Open;
                    CurrentDayPhase = DayPhase;
                    if (DayPhaseChangedTo != null)
                    {
                        DayPhaseChangedTo.Invoke(DayPhase.Open);
                    }
                    break;
                case DayPhase.Open:
                    DayPhase = DayPhase.Night;
                    CurrentDayPhase = DayPhase;
                    if (DayPhaseChangedTo != null)
                    {
                        DayPhaseChangedTo.Invoke(DayPhase.Night);
                    }
                    break;
                case DayPhase.Night:
                    DayPhase = DayPhase.Morning;
                    CurrentDayPhase = DayPhase;
                    if (DayPhaseChangedTo != null)
                    {
                        DayPhaseChangedTo.Invoke(DayPhase.Morning);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
