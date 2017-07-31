using System;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    [Serializable]
    public class PlayerDecisionsState : IState
    {
        public bool AcceptedDrugPushersOffer = true;
        public bool ToldInspectorAboutDrugPusher = false;
        public bool GaveTolstoyDrink = false;
        public int NumberOfDrinksServedInDrugStory = 0;
    }
}
