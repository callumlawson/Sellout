using UnityEngine;

namespace Assets.Scripts.Util
{
    public static class Constants
    {
        //Time
        public static readonly GameTime GameStartTime = new GameTime(1, 0, 0);
        public const float TickPeriodInSeconds = 0.4f;
        public const float SecondsPerGameMinute = 0.6f;
        public const int OpeningHour = 21;
        public const int ClosingHour = 23;

        //Economy
        public const int DrinkSucsessMoney = 5;

        //Drinks
        public const int MaxUnitsInDrink = 6;

        //Player
        public const float PlayerWalkSpeed = 4;
        public const float PlayerRunMultiplier = 1.4f;
        public const float InteractRangeInMeters = 3f;
    }
}
