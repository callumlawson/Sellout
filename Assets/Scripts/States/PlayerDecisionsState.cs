using System;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    [Serializable]
    public class PlayerDecisionsState : IState
    {
        public enum TolstoyAdviceChoices
        {
            Compliment,
            AskOnDate,
            FindFavoriteDrink
        }

        // Drug Story
        public bool AcceptedDrugPushersOffer = true;
        public bool ToldInspectorAboutDrugPusher = false;
        public int NumberOfDrinksServedInDrugStory = 0;

        // Love Story
        public bool GaveTolstoyDrink = false;
        public bool TolstoyAskedToMakeDrink = false;
        public TolstoyAdviceChoices TolstoyAdviceChoice = TolstoyAdviceChoices.Compliment;
        public bool GaveEllieTolstoysDrink = false;
    }
}
