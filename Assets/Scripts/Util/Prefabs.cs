using System.Collections.Generic;

namespace Assets.Scripts.Util
{
    public static class Prefabs
    {
        public const string Person = "Person";
        public const string Booth = "Booth";
        public const string Waypoint = "Waypoint";
        public const string Counter = "Counter";
        public const string Camera = "Camera";
        public const string Player = "Player";
        public const string Console = "Console";
        public const string Drugs = "Drugs";
        public const string Washup = "Washup";

        //Bar objects
        public const string Drink = "Drink";
        public const string GlassStack = "GlassStack";
        public const string MixologyBook = "MixologyBook";
        public const string BeerStack = "BeerStack";
        public const string BarConsole = "BarConsole";
        public const string DrinkSurface = "DrinkSurface";
        public const string Beer = "Beer";
        public const string ServeSpot = "ServeSpot";
        public const string ReceiveSpot = "ReceiveSpot";
        public const string Cubby = "Cubby";
        public const string DispensingBottle = "DispensingBottle";
        public const string IngredientDispenser = "IngredientDispenser";

        //UI
        public const string DiagloguePanelUI = "DialoguePanel";
        public const string BarDiagloguePanelUI = "BarDialoguePanel";
        public const string DialogueLineUI = "DialogueLine";
        public const string ResponseLineUI = "ResponseLine";
        public const string NameTagUI = "NameTag";
        public const string MoodBubbleUI = "MoodBubble";

        public static readonly List<string> BarObjectPrefabs = new List<string>
        {
            Drink,
            GlassStack,
            MixologyBook,
            BeerStack,
            BarConsole,
            DrinkSurface,
            Beer,
            Cubby,
            ServeSpot,
            ReceiveSpot,
            DispensingBottle,
            IngredientDispenser
        };
    }
}