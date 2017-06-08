namespace Assets.Scripts.Util
{
    //Make this reactive so we can change settings at runtime. God mode debug buttons?
    public static class GameSettings
    {
        public static bool IsDebugOn = false;
        public static bool SkipFirstDayFadein = false;
        public static bool DisableTutorial = false;
    }
}
