using UnityEngine;

namespace Assets.Scripts.Util
{
    public static class Constants
    {
        //Time
        public const float TickPeriodInSeconds = 0.4f;
        public const float SecondsPerGameMinute = 0.6f;

        //Economy
        public const int DrinkSucsessMoney = 5;

        //Drinks
        public const int MaxUnitsInDrink = 6;

        //Position
        public static readonly SerializableVector3 OffstagePostion = new Vector3(5.63f, 0.0f, 16.49f);
    }
}
