using UnityEngine;

public class GameTime
{
    private int day;
    private int hour;
    private int minute;

    public GameTime(int day, int hour, int minute)
    {
        this.day = day;
        this.hour = hour;
        this.minute = minute;
    }

    public int GetDay()
    {
        return day;
    }

    public int GetHour()
    {
        return hour;
    }

    public int GetMinute()
    {
        return minute;
    }

    public void IncrementDay()
    {
        day++;
    }

    public void SetTime(int hour, int minute)
    {
        this.hour = hour;
        this.minute = minute;
    }

    public void IncrementMinute()
    {
        minute++;

        if (minute == 60)
        {
            minute = 0;
            hour++;
        }

        if (hour >= 24)
        {
            Debug.LogError("The hour of the day is more than 23 which is unsupported!");
        }
    }

    public GameTime GetCopy()
    {
        return new GameTime(day, hour, minute);
    }

    public static int operator -(GameTime time1, GameTime time2)
    {
        var time1Minutes = time1.hour * 60 + time1.minute;
        var time2Minutes = time2.hour * 60 + time2.minute;
        return time1Minutes - time2Minutes;
    }

    public static bool operator ==(GameTime time1, GameTime time2)
    {
        if (object.ReferenceEquals(time1, null))
        {
            return object.ReferenceEquals(time2, null);
        }

        return time1.GetDay() == time2.GetDay() &&
            time1.GetHour() == time2.GetHour() &&
            time1.GetMinute() == time2.GetMinute();
    }

    public static bool operator !=(GameTime time1, GameTime time2)
    {
        return !(time1 == time2);
    }

    public static bool operator >(GameTime time1, GameTime time2)
    {
        if (time1 == null && time2 == null)
        {
            return false;
        }

        if (time1 == null)
        {
            return false;
        }

        if (time2 == null)
        {
            return true;
        }

        int time1TotalMinutes = time1.day * 24 * 60 + time1.hour * 60 + time1.minute;
        int time2TotalMinutes = time2.day * 24 * 60 + time2.hour * 60 + time2.minute;
        return time1TotalMinutes > time2TotalMinutes;
    }

    public static bool operator <(GameTime time1, GameTime time2)
    {
        return (time1 != time2) && !(time1 > time2);
    }

    public static bool operator >=(GameTime time1, GameTime time2)
    {
        return (time1 == time2) || (time1 > time2);
    }

    public static bool operator <=(GameTime time1, GameTime time2)
    {
        return (time1 == time2) || (time1 < time2);
    }

    public override bool Equals(object obj)
    {
        var item = obj as GameTime;

        if (item == null)
        {
            return false;
        }

        return this == item;
    }
}
