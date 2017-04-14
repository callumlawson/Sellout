using System;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public static class Constants
    {
        //Time
        public static DateTime GameStartTime = new DateTime(2050, 1, 2, 13, 30, 0);
        public const float TickPeriodInSeconds = 0.4f;
        public const float SecondsPerGameMinute = 0.6f;
        public const int DayEndHour = 21; //9pm
        public const int NightLengthInHours = 14; //Till 11am

        //Economy
        public const int DrinkSucsessMoney = 5;

        //Drinks
        public const int MaxUnitsInDrink = 6;
    }
}
